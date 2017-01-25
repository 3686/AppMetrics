﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Options;
using App.Metrics.Gauge.Interfaces;
using App.Metrics.Interfaces;
using App.Metrics.Registry.Interfaces;

namespace App.Metrics.Gauge
{
    internal class DefaultGaugeManager : IMeasureGaugeMetrics
    {
        private readonly IBuildGaugeMetrics _gaugeBuilder;
        private readonly IMetricsRegistry _registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultGaugeManager" /> class.
        /// </summary>
        /// <param name="gaugeBuilder">The gauge builder.</param>
        /// <param name="registry">The registry storing all metric data.</param>
        public DefaultGaugeManager(IBuildGaugeMetrics gaugeBuilder, IMetricsRegistry registry)
        {
            _registry = registry;
            _gaugeBuilder = gaugeBuilder;
        }

        /// <inheritdoc />
        public void SetValue(GaugeOptions options, Func<double> valueProvider) { _registry.Gauge(options, () => _gaugeBuilder.Build(valueProvider)); }

        /// <inheritdoc />
        public void SetValue(GaugeOptions options, Func<IMetricValueProvider<double>> valueProvider) { _registry.Gauge(options, valueProvider); }
    }
}