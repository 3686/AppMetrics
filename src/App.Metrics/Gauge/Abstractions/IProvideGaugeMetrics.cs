﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.MetricTypes;
using App.Metrics.Core.Abstractions;
using App.Metrics.Core.Options;
using App.Metrics.Tagging;

namespace App.Metrics.Gauge.Abstractions
{
    public interface IProvideGaugeMetrics
    {
        /// <summary>
        ///     Records <see cref="IGaugeMetric" /> which is a point in time instantaneous value
        /// </summary>
        /// <param name="options">The details of the gauge that is being measured.</param>
        /// <param name="valueProvider">A function that returns custom value provider for the gauge.</param>
        void Instance(GaugeOptions options, Func<IMetricValueProvider<double>> valueProvider);

        /// <summary>
        ///     Records <see cref="IGaugeMetric" /> which is a point in time instantaneous value
        /// </summary>
        /// <param name="options">The details of the gauge that is being measured.</param>
        /// <param name="tags">
        ///     The runtime tags to set in addition to those defined on the options, this will create a separate metric per unique <see cref="MetricTags"/>
        /// </param>
        /// <param name="valueProvider">A function that returns custom value provider for the gauge.</param>
        void Instance(GaugeOptions options, MetricTags tags, Func<IMetricValueProvider<double>> valueProvider);
    }
}