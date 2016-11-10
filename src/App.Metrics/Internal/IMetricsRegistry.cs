// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Data;

namespace App.Metrics.Internal
{
    internal interface IMetricsRegistry
    {
        bool AddContext(string context, IMetricContextRegistry registry);

        void Clear();

        ICounter Counter<T>(CounterOptions options, Func<T> builder) where T : ICounterMetric;

        void Gauge(GaugeOptions options, Func<IMetricValueProvider<double>> valueProvider);

        Task<MetricsDataValueSource> GetDataAsync(IMetricsFilter filter);

        IHistogram Histogram<T>(HistogramOptions options, Func<T> builder) where T : IHistogramMetric;

        IMeter Meter<T>(MeterOptions options, Func<T> builder) where T : IMeterMetric;

        void RemoveContext(string context);

        ITimer Timer<T>(TimerOptions options, Func<T> builder) where T : ITimerMetric;
    }
}