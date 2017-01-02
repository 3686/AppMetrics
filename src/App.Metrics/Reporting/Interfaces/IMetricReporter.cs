﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Data;

namespace App.Metrics.Reporting.Interfaces
{
    public interface IMetricReporter : IDisposable
    {
        string Name { get; }

        TimeSpan ReportInterval { get; }

        Task<bool> EndAndFlushReportRunAsync(IMetrics metrics);

        void EndMetricTypeReport(Type metricType);

        void ReportEnvironment(EnvironmentInfo environmentInfo);

        void ReportHealth(GlobalMetricTags globalTags,
            IEnumerable<HealthCheck.Result> healthyChecks,
            IEnumerable<HealthCheck.Result> degradedChecks,
            IEnumerable<HealthCheck.Result> unhealthyChecks);

        void ReportMetric<T>(string context, MetricValueSource<T> valueSource);

        void StartMetricTypeReport(Type metricType);

        void StartReportRun(IMetrics metrics);
    }
}