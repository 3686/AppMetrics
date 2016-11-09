// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Infrastructure;

namespace App.Metrics.Reporting
{
    internal class DefaultReportGenerator
    {
        public async Task Generate(IMetricReporter reporter, 
            IMetrics metrics, 
            IMetricsFilter filter, 
            MetricTags globalTags,
            CancellationToken token)
        {
            var reportEnvironment = true;
            var reportHealthChecks = true;

            reporter.StartReport(metrics);

            var data = await metrics.Advanced.DataManager.GetAsync();

            if (filter != default(IMetricsFilter))
            {
                data = data.Filter(filter);
                reportEnvironment = filter.ReportEnvironment;
                reportHealthChecks = filter.ReportHealthChecks;
            }

            if (data.Environment.Entries.Any() && reportEnvironment)
            {
                reporter.StartMetricTypeReport(typeof(EnvironmentInfo));

                reporter.ReportEnvironment(data.Environment);

                reporter.EndMetricTypeReport(typeof(EnvironmentInfo));
            }            

            foreach (var contextValueSource in data.Contexts)
            {
                ReportMetricType(reporter, contextValueSource.Counters,
                    c => { reporter.ReportMetric($"{contextValueSource.Context}", c, globalTags); }, token);

                ReportMetricType(reporter, contextValueSource.Gauges,
                    g => { reporter.ReportMetric($"{contextValueSource.Context}", g, globalTags); }, token);

                ReportMetricType(reporter, contextValueSource.Histograms,
                    h => { reporter.ReportMetric($"{contextValueSource.Context}", h, globalTags); }, token);

                ReportMetricType(reporter, contextValueSource.Meters,
                    m => { reporter.ReportMetric($"{contextValueSource.Context}", m, globalTags); }, token);

                ReportMetricType(reporter, contextValueSource.Timers,
                    t => { reporter.ReportMetric($"{contextValueSource.Context}", t, globalTags); }, token);
            }

            if (reportHealthChecks)
            {
                var healthStatus = await metrics.Advanced.HealthCheckManager.GetStatusAsync();

                reporter.StartMetricTypeReport(typeof(HealthStatus));

                var passed = healthStatus.Results.Where(r => r.Check.IsHealthy);
                var failed = healthStatus.Results.Where(r => !r.Check.IsHealthy);

                reporter.ReportHealth(passed, failed);

                reporter.EndMetricTypeReport(typeof(HealthStatus));
            }

            reporter.EndReport(metrics);
        }

        private static void ReportMetricType<T>(IMetricReporter reporter, IEnumerable<T> metrics, Action<T> report, CancellationToken token)
        {
            var reportingMetrics = metrics.ToList();

            if (token.IsCancellationRequested || !reportingMetrics.Any())
            {
                return;
            }

            reporter.StartMetricTypeReport(typeof(T));

            foreach (var metric in reportingMetrics)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                report(metric);
            }

            reporter.EndMetricTypeReport(typeof(T));
        }
    }
}