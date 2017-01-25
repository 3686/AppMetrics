﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using App.Metrics.Abstractions.Metrics;
using App.Metrics.Apdex;
using App.Metrics.Apdex.Interfaces;
using App.Metrics.Configuration;
using App.Metrics.Core.Options;
using App.Metrics.Counter;
using App.Metrics.Counter.Interfaces;
using App.Metrics.Data;
using App.Metrics.Data.Interfaces;
using App.Metrics.Filtering.Interfaces;
using App.Metrics.Histogram;
using App.Metrics.Histogram.Interfaces;
using App.Metrics.Infrastructure;
using App.Metrics.Internal;
using App.Metrics.Meter;
using App.Metrics.Meter.Interfaces;
using App.Metrics.Registry.Interfaces;
using App.Metrics.Timer;
using App.Metrics.Timer.Interfaces;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Registry.Internal
{
    internal sealed class DefaultMetricsRegistry : IMetricsRegistry
    {
        private readonly IClock _clock;
        private readonly ConcurrentDictionary<string, IMetricContextRegistry> _contexts = new ConcurrentDictionary<string, IMetricContextRegistry>();
        private readonly string _defaultContextLabel;
        private readonly EnvironmentInfoProvider _environmentInfoProvider;
        private readonly ILogger _logger;
        private readonly Func<string, IMetricContextRegistry> _newContextRegistry;
        private readonly Lazy<NullMetricsRegistry> _nullMetricsRegistry = new Lazy<NullMetricsRegistry>();

        public DefaultMetricsRegistry(
            ILoggerFactory loggerFactory,
            AppMetricsOptions options,
            IClock clock,
            EnvironmentInfoProvider environmentInfoProvider,
            Func<string, IMetricContextRegistry> newContextRegistry)
        {
            _logger = loggerFactory.CreateLogger<DefaultMetricContextRegistry>();
            _environmentInfoProvider = environmentInfoProvider;
            _clock = clock;
            _newContextRegistry = newContextRegistry;
            _defaultContextLabel = options.DefaultContextLabel;
            _contexts.TryAdd(_defaultContextLabel, newContextRegistry(_defaultContextLabel));
        }

        public bool AddContext(string context, IMetricContextRegistry registry)
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.AddContext(context, registry);
            }

            if (context.IsMissing())
            {
                throw new ArgumentException("Registry Context cannot be null or empty", nameof(context));
            }

            var attached = _contexts.GetOrAdd(context, registry);

            return ReferenceEquals(attached, registry);
        }

        public IApdex Apdex<T>(ApdexOptions options, Func<T> builder)
            where T : IApdexMetric
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                return _nullMetricsRegistry.Value.Apdex(options, builder);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            return contextRegistry.Apdex(options, builder);
        }

        public void Clear()
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.Clear();
            }

            ForAllContexts(
                c =>
                {
                    c.ClearAllMetrics();
                    _contexts.TryRemove(c.Context, out c);
                });
        }

        public ICounter Counter<T>(CounterOptions options, Func<T> builder)
            where T : ICounterMetric
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                return _nullMetricsRegistry.Value.Counter(options, builder);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            return contextRegistry.Counter(options, builder);
        }

        /// <inheritdoc />
        public void Disable()
        {
            Clear();

            _nullMetricsRegistry.Value.Disable();
        }

        public MetricValueOptions EnsureContextLabel(MetricValueOptions options)
        {
            if (options.Context.IsMissing())
            {
                options.Context = _defaultContextLabel;
            }

            return options;
        }

        public void Gauge(GaugeOptions options, Func<IMetricValueProvider<double>> valueProvider)
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.Gauge(options, valueProvider);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            contextRegistry.Gauge(options, valueProvider);
        }

        public MetricsDataValueSource GetData(IFilterMetrics filter)
        {
            _logger.RetrievedMetricsData();

            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.GetData(filter);
            }

            if (_contexts.Count == 0)
            {
                return MetricsDataValueSource.Empty;
            }

            var environment = _environmentInfoProvider.Build();

            var contexts = _contexts.Values.Select(
                g => new MetricsContextValueSource(
                    g.Context,
                    g.DataProvider.Gauges.ToArray(),
                    g.DataProvider.Counters.ToArray(),
                    g.DataProvider.Meters.ToArray(),
                    g.DataProvider.Histograms.ToArray(),
                    g.DataProvider.Timers.ToArray(),
                    g.DataProvider.ApdexScores.ToArray()));

            var data = new MetricsDataValueSource(_clock.UtcDateTime, environment, contexts);

            _logger.GettingMetricsData();

            return data.Filter(filter);
        }

        public IHistogram Histogram<T>(HistogramOptions options, Func<T> builder)
            where T : IHistogramMetric
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                return _nullMetricsRegistry.Value.Histogram(options, builder);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            return contextRegistry.Histogram(options, builder);
        }

        public IMeter Meter<T>(MeterOptions options, Func<T> builder)
            where T : IMeterMetric
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                return _nullMetricsRegistry.Value.Meter(options, builder);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            return contextRegistry.Meter(options, builder);
        }

        public void RemoveContext(string context)
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.RemoveContext(context);
            }

            if (context.IsMissing())
            {
                throw new ArgumentException("Registry Context cannot be null or empty", nameof(context));
            }

            IMetricContextRegistry contextRegistry;

            if (_contexts.TryRemove(context, out contextRegistry))
            {
                contextRegistry.ClearAllMetrics();
            }
        }

        public ITimer Timer<T>(TimerOptions options, Func<T> builder)
            where T : ITimerMetric
        {
            if (_nullMetricsRegistry.IsValueCreated)
            {
                _nullMetricsRegistry.Value.Timer(options, builder);
            }

            EnsureContextLabel(options);

            var contextRegistry = _contexts.GetOrAdd(options.Context, _newContextRegistry);

            return contextRegistry.Timer(options, builder);
        }

        private void ForAllContexts(Action<IMetricContextRegistry> action)
        {
            foreach (var contextRegistry in _contexts.Values)
            {
                action(contextRegistry);
            }
        }
    }
}