﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Options;
using App.Metrics.Data.Interfaces;
using App.Metrics.Sampling.Interfaces;

// ReSharper disable CheckNamespace

namespace App.Metrics.Core
// ReSharper restore CheckNamespace
{
    public static class AdvancedMetricsContextExtensions
    {
        public static ICounterMetric BuildCounter(this IAdvancedMetrics context, CounterOptions options)
        {
            return new CounterMetric();
        }

        public static IMetricValueProvider<double> BuildGauge(this IAdvancedMetrics context, GaugeOptions options, Func<double> valueProvider)
        {
            return new FunctionGauge(valueProvider);
        }

        public static IHistogramMetric BuildHistogram(this IAdvancedMetrics context, HistogramOptions options)
        {
            return new HistogramMetric(options.SamplingType, options.SampleSize, options.ExponentialDecayFactor);
        }

        public static IHistogramMetric BuildHistogram(this IAdvancedMetrics context, HistogramOptions options, IReservoir reservoir)
        {
            return new HistogramMetric(reservoir);
        }

        public static IMeterMetric BuildMeter(this IAdvancedMetrics context, MeterOptions options)
        {
            return new MeterMetric(context.Clock);
        }

        public static ITimerMetric BuildTimer(this IAdvancedMetrics context, TimerOptions options)
        {
            return new TimerMetric(options.SamplingType, options.SampleSize, options.ExponentialDecayFactor, context.Clock);
        }

        public static ITimerMetric BuildTimer(this IAdvancedMetrics context, TimerOptions options, IHistogramMetric histogram)
        {
            return new TimerMetric(histogram, context.Clock);
        }

        public static ITimerMetric BuildTimer(this IAdvancedMetrics context, TimerOptions options, IReservoir reservoir)
        {
            return new TimerMetric(reservoir, context.Clock);
        }
    }
}