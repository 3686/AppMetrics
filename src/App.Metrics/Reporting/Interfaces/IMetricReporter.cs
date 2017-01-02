﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Data;
using App.Metrics.Utils;

namespace App.Metrics.Reporting.Interfaces
{
    public interface IMetricReporter : IHideObjectMembers, IDisposable
    {
        string Name { get; }

        TimeSpan ReportInterval { get; }

        void EndMetricTypeReport(Type metricType);

        Task<bool> EndReportAsync(IMetrics metrics);

        void ReportEnvironment(EnvironmentInfo environmentInfo);

        void ReportHealth(GlobalMetricTags globalTags,
            IEnumerable<HealthCheck.Result> healthyChecks, 
            IEnumerable<HealthCheck.Result> degradedChecks, 
            IEnumerable<HealthCheck.Result> unhealthyChecks);

        void ReportMetric<T>(string context, MetricValueSource<T> valueSource);

        void StartMetricTypeReport(Type metricType);

        void StartReport(IMetrics metrics);
    }
}