﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Configuration;
using App.Metrics.Core;
using App.Metrics.Core.Options;
using App.Metrics.Data;
using App.Metrics.Infrastructure;
using App.Metrics.Interfaces;
using App.Metrics.Internal;
using App.Metrics.Internal.Builders;
using App.Metrics.Internal.Managers;
using App.Metrics.Utils;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Facts.Fixtures
{
    public class MetricsReportingFixture : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        public MetricsReportingFixture()
        {
            var healthFactoryLogger = _loggerFactory.CreateLogger<HealthCheckFactory>();
            var options = new AppMetricsOptions { DefaultSamplingType = SamplingType.LongTerm };
            var clock = new TestClock();
            Func<string, IMetricContextRegistry> newContextRegistry = name => new DefaultMetricContextRegistry(name);
            var registry = new DefaultMetricsRegistry(_loggerFactory, options, clock, new EnvironmentInfoProvider(), newContextRegistry);
            var healthCheckFactory = new HealthCheckFactory(healthFactoryLogger);
            var metricBuilderFactory = new DefaultMetricsBuilder();
            var filter = new DefaultMetricsFilter();
            var healthManager = new DefaultHealthManager(_loggerFactory.CreateLogger<DefaultHealthManager>(), healthCheckFactory);
            var dataManager = new DefaultDataManager(
                filter,
                registry);

            var metricsManagerFactory = new DefaultMetricsManagerFactory(registry, metricBuilderFactory, clock);
            var metricsManagerAdvancedFactory = new DefaultMetricsAdvancedManagerFactory(registry, metricBuilderFactory, clock);
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
                healthManager);

            RecordSomeMetrics();
        }

        public Func<IMetrics, MetricsDataValueSource> CurrentData =>
            ctx => Metrics.Data.ReadData();

        public Func<IMetrics, IMetricsFilter, MetricsDataValueSource> CurrentDataWithFilter
            => (ctx, filter) => Metrics.Data.ReadData(filter);

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

        private void RecordSomeMetrics()
        {
            var counterOptions = new CounterOptions
                                 {
                                     Name = "test_counter",
                                     MeasurementUnit = Unit.Requests,
                                     Tags = new MetricTags().With("tag1", "value")
                                 };

            var meterOptions = new MeterOptions
                               {
                                   Name = "test_meter",
                                   MeasurementUnit = Unit.None,
                                   Tags = new MetricTags().With("tag2", "value")
                               };

            var timerOptions = new TimerOptions
                               {
                                   Name = "test_timer",
                                   MeasurementUnit = Unit.Requests
                               };

            var histogramOptions = new HistogramOptions
                                   {
                                       Name = "test_histogram",
                                       MeasurementUnit = Unit.Requests
                                   };

            var gaugeOptions = new GaugeOptions
                               {
                                   Name = "test_gauge"
                               };

            Metrics.Measure.Counter.Increment(counterOptions);
            Metrics.Measure.Meter.Mark(meterOptions);
            Metrics.Measure.Timer.Time(timerOptions, () => Metrics.Clock.Advance(TimeUnit.Milliseconds, 10));
            Metrics.Measure.Histogram.Update(histogramOptions, 5);
            Metrics.Measure.Gauge.SetValue(gaugeOptions, () => 8);
        }
    }
}