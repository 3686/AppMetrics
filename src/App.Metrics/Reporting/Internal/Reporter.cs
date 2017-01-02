// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.DependencyInjection.Internal;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Reporting.Internal
{
    internal sealed class Reporter : IReporter
    {
        private readonly ILogger<Reporter> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly Dictionary<Type, IMetricReporter> _metricReporters;
        private readonly Dictionary<Type, IReporterProvider> _providers;
        private readonly DefaultReportGenerator _reportGenerator;
        private readonly IScheduler _scheduler;

        public Reporter(ReportFactory reportFactory, IScheduler scheduler, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _reportGenerator = new DefaultReportGenerator();
            _scheduler = scheduler;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Reporter>();

            _providers = reportFactory.GetProviders();

            if (_providers.Count <= 0)
            {
                return;
            }

            _metricReporters = new Dictionary<Type, IMetricReporter>(_providers.Count);

            foreach (var provider in _providers)
            {
                _metricReporters.Add(provider.Key, provider.Value.CreateMetricReporter(provider.Key.Name, _loggerFactory));
            }
        }

        public void RunReports(IMetrics context, CancellationToken token)
        {
            if (_metricReporters == null)
            {
                return;
            }

            List<Exception> exceptions = null;
            var reportTasks = new List<Task>();

            foreach (var metricReporter in _metricReporters)
            {
                var logger = _loggerFactory.CreateLogger(metricReporter.Value.GetType());

                var provider = _providers[metricReporter.Key];
                var settings = provider.Settings;

                logger.ReportRunning(metricReporter.Value);

                reportTasks.Add(ScheduleReport(context, token, metricReporter, logger, provider).WithAggregateException());
            }

            try
            {
                Task.WaitAll(reportTasks.ToArray(), token);
            }
            catch (Exception ex)
            {
                //TODO: AH - confirm exception handlling and log
                exceptions = new List<Exception> { ex };
            }

            if (exceptions != null && exceptions.Count > 0)
            {
                var aggregateException = new AggregateException(
                    message: "An error occurred while running reporter(s).",
                    innerExceptions: exceptions);

                _logger.ReportRunFailed(aggregateException);

                throw aggregateException;
            }
        }

        private Task ScheduleReport(IMetrics context, CancellationToken token, KeyValuePair<Type, IMetricReporter> metricReporter, 
            ILogger logger, IReporterProvider provider)
        {
            return _scheduler.Interval(metricReporter.Value.ReportInterval, TaskCreationOptions.LongRunning, async() =>
                {
                    var startTimestamp = _logger.IsEnabled(LogLevel.Information) ? Stopwatch.GetTimestamp() : 0;

                    logger.ReportedStarted(metricReporter.Value);

                    await _reportGenerator.GenerateAsync(metricReporter.Value, context, provider.Filter, token);

                    logger.ReportRan(metricReporter.Value, startTimestamp);
                },
                token);
        }
    }
}