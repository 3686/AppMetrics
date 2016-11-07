﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using App.Metrics.Core;
using App.Metrics.Data;
using App.Metrics.Infrastructure;
using App.Metrics.Utils;

namespace App.Metrics.Reporting
{
    public interface IMetricReporter : IHideObjectMembers, IDisposable
    {
        //TODO: AH - should report interval live here?
        TimeSpan ReportInterval { get; }

        void EndMetricTypeReport(Type metricType);

        void EndReport(IMetricsContext metricsContext);

        void ReportEnvironment(EnvironmentInfo environmentInfo);

        void ReportHealth(IEnumerable<HealthCheck.Result> healthyChecks, IEnumerable<HealthCheck.Result> unhealthyChecks);

        void ReportMetric<T>(string name, MetricValueSource<T> valueSource, MetricTags globalTags);

        void StartMetricTypeReport(Type metricType);

        void StartReport(IMetricsContext metricsContext);
    }
}