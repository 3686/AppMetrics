﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Options;
using App.Metrics.Interfaces;
using App.Metrics.Utils;

namespace App.Metrics.Internal.Managers
{
    public class DefaultTimerAdvancedManager : IMeasureTimerMetricsAdvanced
    {
        private readonly IClock _clock;
        private readonly IMetricsRegistry _registry;
        private readonly IBuildTimerMetrics _timerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTimerAdvancedManager" /> class.
        /// </summary>
        /// <param name="timerBuilder">The timer builder.</param>
        /// <param name="registry">The metrics registry.</param>
        /// <param name="clock">The clock.</param>
        public DefaultTimerAdvancedManager(IBuildTimerMetrics timerBuilder, IMetricsRegistry registry, IClock clock)
        {
            _registry = registry;
            _clock = clock;
            _timerBuilder = timerBuilder;
        }

        /// <inheritdoc />
        public ITimer With(TimerOptions options)
        {
            if (options.WithReservoir != null)
            {
                return With(
                    options,
                    () => _timerBuilder.Instance(options.WithReservoir(), _clock));
            }

            return _registry.Timer(
                options,
                () => _timerBuilder.Instance(options.SamplingType, options.SampleSize, options.ExponentialDecayFactor, _clock));
        }

        /// <inheritdoc />
        public ITimer With<T>(TimerOptions options, Func<T> builder)
            where T : ITimerMetric { return _registry.Timer(options, builder); }

        /// <inheritdoc />
        public ITimer WithHistogram<T>(TimerOptions options, Func<T> histogramMetricBuilder)
            where T : IHistogramMetric
        {
            return With(
                options,
                () => _timerBuilder.Instance(histogramMetricBuilder(), _clock));
        }
    }
}