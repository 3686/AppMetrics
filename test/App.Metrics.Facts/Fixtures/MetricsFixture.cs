// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Configuration;
using App.Metrics.Core;
using App.Metrics.Data;
using App.Metrics.Infrastructure;
using App.Metrics.Interfaces;
using App.Metrics.Internal;
using App.Metrics.Internal.Builders;
using App.Metrics.Internal.Managers;
using App.Metrics.Internal.Providers;
using App.Metrics.Utils;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Facts.Fixtures
{
    public class MetricsFixture : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        public MetricsFixture()
        {
            var healthFactoryLogger = _loggerFactory.CreateLogger<HealthCheckFactory>();
            var clock = new TestClock();
            var options = new AppMetricsOptions { DefaultSamplingType = SamplingType.LongTerm };
            Func<string, IMetricContextRegistry> newContextRegistry = name => new DefaultMetricContextRegistry(name);
            var registry = new DefaultMetricsRegistry(_loggerFactory, options, clock, new EnvironmentInfoProvider(), newContextRegistry);
            var healthCheckFactory = new HealthCheckFactory(healthFactoryLogger);
            var metricBuilderFactory = new DefaultMetricsBuilderFactory();
            var filter = new DefaultMetricsFilter();
            var dataManager = new DefaultMetricValuesProvider(filter, registry);
            var healthStatusProvider = new DefaultHealthProvider(_loggerFactory.CreateLogger<DefaultHealthProvider>(), healthCheckFactory);
            var metricsManagerFactory = new DefaultMeasureMetricsProvider(registry, metricBuilderFactory, clock);
            var metricsManagerAdvancedFactory = new DefaultMetricsProvider(registry, metricBuilderFactory, clock);
            var metricsManager = new DefaultMetricsManager(registry, _loggerFactory.CreateLogger<DefaultMetricsManager>());
            Metrics = new DefaultMetrics(
                options,
                clock,
                filter,
                metricsManagerFactory,
                metricBuilderFactory,
                metricsManagerAdvancedFactory,
                dataManager,
                metricsManager,
                healthStatusProvider);
        }

        public Func<IMetrics, MetricsDataValueSource> CurrentData =>
            ctx => Metrics.Snapshot.Get();

        public Func<IMetrics, IFilterMetrics, MetricsDataValueSource> CurrentDataWithFilter
            => (ctx, filter) => Metrics.Snapshot.Get(filter);

        public IMetrics Metrics { get; }

        public void Dispose() { Dispose(true); }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Metrics?.Manage.Reset();
        }
    }
}