﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Configuration;
using App.Metrics.Core;
using App.Metrics.Core.Abstractions;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Internal;
using App.Metrics.Core.Options;
using App.Metrics.Filtering;
using App.Metrics.Health.Abstractions;
using App.Metrics.Health.Internal;
using App.Metrics.Infrastructure;
using App.Metrics.Registry.Abstractions;
using App.Metrics.Registry.Internal;
using App.Metrics.Tagging;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Facts.Fixtures
{
    public class MetricsReportingFixture : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        public MetricsReportingFixture()
        {
            var healthFactoryLogger = _loggerFactory.CreateLogger<HealthCheckFactory>();
            var options = new AppMetricsOptions();
            var clock = new TestClock();

            IMetricContextRegistry NewContextRegistry(string name) => new DefaultMetricContextRegistry(name);

            var registry = new DefaultMetricsRegistry(_loggerFactory, options, clock, new EnvironmentInfoProvider(), NewContextRegistry);
            var healthCheckFactory = new HealthCheckFactory(healthFactoryLogger);
            var metricBuilderFactory = new DefaultMetricsBuilderFactory();
            var filter = new DefaultMetricsFilter();
            var dataManager = new DefaultMetricValuesProvider(
                filter,
                registry);


            var metricsManagerFactory = new DefaultMeasureMetricsProvider(registry, metricBuilderFactory, clock);
            var metricsManagerAdvancedFactory = new DefaultMetricsProvider(registry, metricBuilderFactory, clock);
            var metricsManager = new DefaultMetricsManager(registry, _loggerFactory.CreateLogger<DefaultMetricsManager>());

            var samplesRun = false;
            
            Metrics = () =>
            {
                var healthManager = new DefaultHealthProvider(new Lazy<IMetrics>(Metrics), _loggerFactory.CreateLogger<DefaultHealthProvider>(), healthCheckFactory);
                var defaultMetrics = new DefaultMetrics(
                    clock,
                    filter,
                    metricsManagerFactory,
                    metricBuilderFactory,
                    metricsManagerAdvancedFactory,
                    dataManager,
                    metricsManager,
                    healthManager);

                if (!samplesRun)
                {
                    RecordSomeMetrics(defaultMetrics);
                    samplesRun = true;
                }

                return defaultMetrics;
            };
        }


        public Func<IMetrics> Metrics { get; }

        public void Dispose() { Dispose(true); }

        protected virtual void Dispose(bool disposing)
        {
        }

        private void RecordSomeMetrics(IMetrics metrics)
        {
            var counterOptions = new CounterOptions
                                 {
                                     Name = "test_counter",
                                     MeasurementUnit = Unit.Requests,
                                     Tags = new MetricTags("tag1", "value")
                                 };

            var meterOptions = new MeterOptions
                               {
                                   Name = "test_meter",
                                   MeasurementUnit = Unit.None,
                                   Tags = new MetricTags("tag2", "value")
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


            metrics.Measure.Counter.Increment(counterOptions);
            metrics.Measure.Meter.Mark(meterOptions);
            metrics.Measure.Timer.Time(timerOptions, () => metrics.Clock.Advance(TimeUnit.Milliseconds, 10));
            metrics.Measure.Histogram.Update(histogramOptions, 5);
            metrics.Measure.Gauge.SetValue(gaugeOptions, () => 8);
        }
    }
}