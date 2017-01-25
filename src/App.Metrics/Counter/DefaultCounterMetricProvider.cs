﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Abstractions.Metrics;
using App.Metrics.Core.Options;
using App.Metrics.Counter.Interfaces;
using App.Metrics.Interfaces;
using App.Metrics.Registry.Interfaces;

namespace App.Metrics.Counter
{
    public class DefaultCounterMetricProvider : IProvideCounterMetrics
    {
        private readonly IBuildCounterMetrics _counterBuilder;
        private readonly IMetricsRegistry _registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultCounterMetricProvider" /> class.
        /// </summary>
        /// <param name="counterBuilder">The counter builder.</param>
        /// <param name="registry">The registry.</param>
        public DefaultCounterMetricProvider(IBuildCounterMetrics counterBuilder, IMetricsRegistry registry)
        {
            _registry = registry;
            _counterBuilder = counterBuilder;
        }

        /// <inheritdoc />
        public ICounter Instance<T>(CounterOptions options, Func<T> builder)
            where T : ICounterMetric { return _registry.Counter(options, builder); }

        /// <inheritdoc />
        public ICounter Instance(CounterOptions options) { return Instance(options, () => _counterBuilder.Build()); }
    }
}