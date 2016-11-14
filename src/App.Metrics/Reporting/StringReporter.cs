// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Metrics.Core;
using App.Metrics.Data;
using App.Metrics.Formatting.Humanize;
using App.Metrics.Infrastructure;

namespace App.Metrics.Reporting
{
    public sealed class StringReporter : IMetricReporter
    {
        private StringBuilder _buffer;
        private bool _disposed;

        public StringReporter() :
            this("String Reporter")
        {
        }

        public StringReporter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            _buffer = new StringBuilder();
        }

        public string Name { get; }

        public string Result => _buffer.ToString();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.

                    if (_buffer != null)
                    {
                        _buffer.Clear();
                        _buffer = null;
                    }
                }
            }

            _disposed = true;
        }

        public TimeSpan ReportInterval { get; } = TimeSpan.FromSeconds(5);

        public void EndMetricTypeReport(Type metricType)
        {
            _buffer.WriteEndMetricType(metricType);
        }

        public void EndReport(IMetrics metrics)
        {
            _buffer.WriteMetricEndReport(Name,
                metrics.Advanced.Clock.FormatTimestamp(metrics.Advanced.Clock.UtcDateTime));
        }

        public void ReportEnvironment(EnvironmentInfo environmentInfo)
        {
            _buffer.WriteEnvironmentInfo(environmentInfo);
        }

        public void ReportHealth(IEnumerable<HealthCheck.Result> healthyChecks, IEnumerable<HealthCheck.Result> unhealthyChecks)
        {
            var passed = healthyChecks.ToList();
            var failed = unhealthyChecks.ToList();

            _buffer.WriteHealthStatus(!failed.Any());

            _buffer.WritePassedHealthChecksHeader();
            ;
            passed.ForEach(c => _buffer.WriteHealthCheckResult(c));

            _buffer.WriteFailedHealthChecksHeader();

            failed.ForEach(c => _buffer.WriteHealthCheckResult(c));
        }

        public void ReportMetric<T>(string name, MetricValueSource<T> valueSource, MetricTags globalTags)
        {
            _buffer.WriteMetricName(name, valueSource);
            _buffer.WriteMetricValue(valueSource);
        }

        public void StartMetricTypeReport(Type metricType)
        {
            _buffer.WriteStartMetricType(metricType);
        }

        public void StartReport(IMetrics metrics)
        {
            _buffer.WriteMetricStartReport(Name,
                metrics.Advanced.Clock.FormatTimestamp(metrics.Advanced.Clock.UtcDateTime));
        }
    }
}