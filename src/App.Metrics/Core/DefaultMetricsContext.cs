﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Concurrent;
using App.Metrics.DataProviders;
using App.Metrics.Registries;
using App.Metrics.Utils;

namespace App.Metrics.Core
{
    public sealed class DefaultMetricsContext : IMetricsContext
    {
        internal const string InternalMetricsContextName = "App.Metrics.Internal";
        private readonly IMetricsBuilder _builder;
        private readonly IMetricsRegistry _registry;


        public DefaultMetricsContext(string groupName,
            IClock clock,
            SamplingType defaultSamplingType,
            Func<IMetricsRegistry> newRegistry,
            IMetricsBuilder builder,
            IHealthCheckManager healthCheckManager,
            IMetricsDataManager metricsDataManager)
        {
            //TODO: AH - refactor - rather than check for nulls set defaults, features?

            _registry = newRegistry();
            _builder = builder;

            GroupName = groupName;
            Advanced = new DefaultAdancedMetricsContext(this, clock, defaultSamplingType, _registry, newRegistry, builder, healthCheckManager, metricsDataManager);
        }

        public IAdvancedMetricsContext Advanced { get; }

        public string GroupName { get; }

        //TODO: AH - use IReadonlyDictionary but advanced contexts needs to add
        public ConcurrentDictionary<string, IMetricsContext> Groups { get; } = new ConcurrentDictionary<string, IMetricsContext>();

        public void Decrement(CounterOptions options, long amount)
        {
            _registry.Counter(options, () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Decrement(amount);
        }

        public void Decrement(CounterOptions options, string item)
        {
            _registry.Counter(options, () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Decrement(item);
        }

        public void Decrement(CounterOptions options, long amount, string item)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Decrement(item, amount);
        }

        public void Decrement(CounterOptions options)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Decrement();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposing) return;            

            //if (!_isDisabled)
            //{
            //    ContextShuttingDown?.Invoke(this, EventArgs.Empty);
            //}
        }

        public void Gauge(GaugeOptions options, Func<double> valueProvider)
        {
            _registry.Gauge(options,
                () => _builder.BuildGauge(options.Name, options.MeasurementUnit, valueProvider));
        }


        public void Increment(CounterOptions options)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Increment();
        }

        public void Increment(CounterOptions options, long amount)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Increment(amount);
        }

        public void Increment(CounterOptions options, string item)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Increment(item);
        }

        public void Increment(CounterOptions options, long amount, string item)
        {
            _registry.Counter(options,
                    () => _builder.BuildCounter(options.Name, options.MeasurementUnit))
                .Increment(item, amount);
        }

        public void Mark(MeterOptions options)
        {
            _registry.Meter(options,
                    () => _builder.BuildMeter(options.Name, options.MeasurementUnit, options.RateUnit))
                .Mark();
        }

        public void Mark(MeterOptions options, long amount)
        {
            _registry.Meter(options,
                    () => _builder.BuildMeter(options.Name, options.MeasurementUnit, options.RateUnit))
                .Mark(amount);
        }

        public void Mark(MeterOptions options, string item)
        {
            _registry.Meter(options,
                    () => _builder.BuildMeter(options.Name, options.MeasurementUnit, options.RateUnit))
                .Mark(item);
        }

        public void Mark(MeterOptions options, long amount, string item)
        {
            _registry.Meter(options,
                    () => _builder.BuildMeter(options.Name, options.MeasurementUnit, options.RateUnit))
                .Mark(item, amount);
        }

        public void Time(TimerOptions options, Action action)
        {
            using (_registry.Timer(options,
                    () => _builder.BuildTimer(options.Name, options.MeasurementUnit, options.RateUnit, options.DurationUnit, options.SamplingType))
                .NewContext())
            {
                action();
            }
        }

        public void Time(TimerOptions options, Action action, string userValue)
        {
            using (_registry.Timer(options,
                    () => _builder.BuildTimer(options.Name, options.MeasurementUnit, options.RateUnit, options.DurationUnit, options.SamplingType))
                .NewContext(userValue))
            {
                action();
            }
        }

        public void Update(HistogramOptions options, long value)
        {
            _registry.Histogram(options,
                    () => _builder.BuildHistogram(options.Name, options.MeasurementUnit, options.SamplingType))
                .Update(value);
        }

        public void Update(HistogramOptions options, long value, string userValue)
        {
            _registry.Histogram(options,
                    () => _builder.BuildHistogram(options.Name, options.MeasurementUnit, options.SamplingType))
                .Update(value, userValue);
        }
    }
}