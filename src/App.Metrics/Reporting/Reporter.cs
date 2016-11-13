// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Infrastructure;
using App.Metrics.Scheduling;

namespace App.Metrics.Reporting
{
    internal sealed class Reporter : IReporter
    {
        private readonly Dictionary<Type, IMetricReporter> _metricReporters;
        private readonly Dictionary<Type, IReporterProvider> _providers;
        private readonly DefaultReportGenerator _reportGenerator;
        private readonly IScheduler _scheduler;

        public Reporter(ReportFactory reportFactory, IScheduler scheduler)
        {
            _reportGenerator = new DefaultReportGenerator();
            _scheduler = scheduler;

            _providers = reportFactory.GetProviders();

            if (_providers.Count <= 0) return;

            _metricReporters = new Dictionary<Type, IMetricReporter>(_providers.Count);

            foreach (var provider in _providers)
            {
                _metricReporters.Add(provider.Key, provider.Value.CreateMetricReporter(provider.Key.Name));
            }
        }

        public async Task RunReportsAsync(IMetrics context, CancellationToken token)
        {
            if (_metricReporters == null)
            {
                return;
            }

            List<Exception> exceptions = null;

            foreach (var metricReporter in _metricReporters)
            {
                try
                {
                    var provider = _providers[metricReporter.Key];
                    var settings = provider.Settings;
                    var task = _scheduler.Interval(metricReporter.Value.ReportInterval, async () =>
                        await _reportGenerator.Generate(metricReporter.Value, context, provider.Filter,
                            settings.GlobalTags, token), token);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            if (exceptions != null && exceptions.Count > 0)
            {
                throw new AggregateException(
                    message: "An error occurred while running reporter(s).",
                    innerExceptions: exceptions);
            }
            await AppMetricsTaskCache.EmptyTask;
        }
    }
}