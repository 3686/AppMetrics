﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using App.Metrics.Registries;
using App.Metrics.Reporters;
using App.Metrics.Utils;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Internal
{
    internal sealed class DefaultMetricReporterRegistry : IMetricReporterRegistry, IHideObjectMembers, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMetricsContext _metricsContext;
        private readonly List<DefaultScheduledReporter> _reports = new List<DefaultScheduledReporter>();

        private bool _disposed = false;

        public DefaultMetricReporterRegistry(
            IMetricsContext metricsContext,
            ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (metricsContext == null)
            {
                throw new ArgumentNullException(nameof(metricsContext));
            }

            _metricsContext = metricsContext;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DefaultMetricReporterRegistry>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RunReports(CancellationToken token)
        {
            _logger.RunReportsExecuting();

            var startTimestamp = _logger.IsEnabled(LogLevel.Information) ? Stopwatch.GetTimestamp() : 0;

            _reports.ForEach(r => r.Start(token));

            _logger.RunReportsExecuted(startTimestamp);
        }

        /// <summary>
        ///     Stop all registered reports and clear the registrations.
        /// </summary>
        public void StopAndClearAllReports()
        {
            if (_reports == null) return;

            _reports.ForEach(r =>
            {
                if (r != default(DefaultScheduledReporter))
                {
                    r.Dispose();
                }
            });

            _reports.Clear();
        }

        /// <summary>
        ///     Schedule a Console Report to be executed and displayed on the console at a fixed <paramref name="interval" />.
        /// </summary>
        /// <param name="interval">Interval at which to display the report on the Console.</param>
        /// <param name="filter">Only report metrics that match the filter.</param>
        public IMetricReporterRegistry WithConsoleReport(TimeSpan interval, IMetricsFilter filter = null)
        {
            return WithReport(new ConsoleReport(_metricsContext, filter, _loggerFactory), interval, filter);
        }

        /// <summary>
        ///     Schedule a generic reporter to be executed at a fixed <paramref name="interval" />
        /// </summary>
        /// <param name="report">Function that returns an instance of a reporter</param>
        /// <param name="interval">Interval at which to run the report.</param>
        /// <param name="filter">Only report metrics that match the filter.</param>
        public IMetricReporterRegistry WithReport(IMetricsReport report, TimeSpan interval,
            IMetricsFilter filter = null)
        {
            var newReport = new DefaultScheduledReporter(_loggerFactory, _metricsContext, report, interval);
            _reports.Add(newReport);
            return this;
        }

        /// <summary>
        ///     Schedule a Human Readable report to be executed and appended to a text file.
        /// </summary>
        /// <param name="filePath">File where to append the report.</param>
        /// <param name="interval">Interval at which to run the report.</param>
        /// <param name="filter">Only report metrics that match the filter.</param>
        public IMetricReporterRegistry WithTextFileReport(string filePath, TimeSpan interval,
            IMetricsFilter filter = null)
        {
            return WithReport(new TextFileReport(filePath, _loggerFactory, filter,
                _metricsContext.Advanced.Clock), interval, filter);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                }

                StopAndClearAllReports();
            }

            _disposed = true;
        }
    }
}