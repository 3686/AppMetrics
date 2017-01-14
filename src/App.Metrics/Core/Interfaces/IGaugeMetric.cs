﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.Data.Interfaces;

namespace App.Metrics.Core.Interfaces
{
    /// <summary>
    ///     Provides access to a gauge metric implementation e.g. <see cref="FunctionGauge" />, <see cref="HitRatioGauge" />,
    ///     allows custom gauges to be implemented
    /// </summary>
    /// <seealso cref="IGaugeMetric" />
    public interface IGaugeMetric : IMetricValueProvider<double>
    {
    }
}