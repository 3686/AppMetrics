﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Options;
using App.Metrics.Utils;

namespace App.Metrics
{
    public interface IMetrics : IHideObjectMembers
    {
        IAdvancedMetrics Advanced { get; }

        void Decrement(CounterOptions options);

        void Decrement(CounterOptions options, long amount);

        void Decrement(CounterOptions options, string item);

        void Decrement(CounterOptions options, long amount, string item);

        void Decrement(CounterOptions options, Action<MetricItem> item);

        void Decrement(CounterOptions options, long amount, Action<MetricItem> item);

        void Gauge(GaugeOptions options, Func<double> valueProvider);

        void Increment(CounterOptions options);

        void Increment(CounterOptions options, long amount);

        void Increment(CounterOptions options, string item);

        void Increment(CounterOptions options, long amount, string item);

        void Mark(MeterOptions options, long amount);

        void Mark(MeterOptions options, string item);

        void Mark(MeterOptions options, Action<MetricItem> item);

        void Mark(MeterOptions options, long amount, string item);

        void Mark(MeterOptions options, long amount, Action<MetricItem> item);

        void Mark(MeterOptions options);

        void Time(TimerOptions options, Action action);

        void Time(TimerOptions options, Action action, string userValue);

        TimerContext Time(TimerOptions options, string userValue);

        TimerContext Time(TimerOptions options);

        void Update(HistogramOptions options, long value);

        void Update(HistogramOptions options, long value, string userValue);
    }
}