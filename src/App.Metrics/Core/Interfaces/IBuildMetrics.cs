﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.Abstractions.Metrics;
using App.Metrics.Abstractions.MetricTypes;
using App.Metrics.Apdex.Interfaces;
using App.Metrics.Gauge.Interfaces;
using App.Metrics.Histogram.Interfaces;
using App.Metrics.Meter.Interfaces;
using App.Metrics.Registry.Interfaces;
using App.Metrics.Timer.Interfaces;

namespace App.Metrics.Core.Interfaces
{
    /// <summary>
    ///     Provides access to APIs which build instances of all available metric types. Metrics created are not added to the
    ///     <see cref="IMetricsRegistry" />.
    /// </summary>
    public interface IBuildMetrics
    {
        /// <summary>
        ///     Gets the Apdex API to build <see cref="IApdexMetric" />s
        /// </summary>
        /// <value>
        ///     The Apdex API for building <see cref="IApdexMetric" />s
        /// </value>
        IBuildApdexMetrics Apdex { get; }

        /// <summary>
        ///     Gets the Counter API to build <see cref="ICounterMetric" />s
        /// </summary>
        /// <value>
        ///     The Counter API for building <see cref="ICounterMetric" />s
        /// </value>
        IBuildCounterMetrics Counter { get; }

        /// <summary>
        ///     Gets the Gauge API to build <see cref="IGaugeMetric" />s
        /// </summary>
        /// <value>
        ///     The Gauge API for building <see cref="IGaugeMetric" />s
        /// </value>
        IBuildGaugeMetrics Gauge { get; }

        /// <summary>
        ///     Gets the Histogram API to build <see cref="IHistogramMetric" />s
        /// </summary>
        /// <value>
        ///     The Histogram API for building <see cref="IHistogramMetric" />s
        /// </value>
        IBuildHistogramMetrics Histogram { get; }

        /// <summary>
        ///     Gets the Meter API to build <see cref="IMeterMetric" />s
        /// </summary>
        /// <value>
        ///     The Meter API for building <see cref="IMeterMetric" />s
        /// </value>
        IBuildMeterMetrics Meter { get; }

        /// <summary>
        ///     Gets the Timer API to build <see cref="ITimerMetric" />s
        /// </summary>
        /// <value>
        ///     The Timer API for building <see cref="ITimerMetric" />s
        /// </value>
        IBuildTimerMetrics Timer { get; }
    }
}