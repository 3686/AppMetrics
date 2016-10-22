﻿using System.Linq;
using App.Metrics;
using App.Metrics.Core;
using App.Metrics.DataProviders;
using App.Metrics.Health;
using App.Metrics.MetricData;
using App.Metrics.Registries;
using App.Metrics.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Metrics.Facts
{
    public static class TestContextHelper
    {
        private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        public static IMetricsContext Instance(string context, IClock clock, IScheduler scheduler)
        {
            return new DefaultMetricsContext(context, clock, SamplingType.ExponentiallyDecaying, () => new DefaultMetricsRegistry(),
                new TestMetricsBuilder(clock, scheduler),
                new DefaultHealthCheckManager(LoggerFactory, new HealthCheckRegistry(LoggerFactory, Enumerable.Empty<HealthCheck>(), Options.Create(new AppMetricsOptions()))),
                new DefaultMetricsDataManager(LoggerFactory, clock, Enumerable.Empty<EnvironmentInfoEntry>()));
        }

        public static IMetricsContext Instance()
        {
            return Instance("TestContext", new Clock.TestClock());
        }

        public static IMetricsContext Instance(IClock clock, IScheduler scheduler)
        {
            return Instance("TestContext", clock, scheduler);
        }

        public static IMetricsContext Instance(string context, IClock clock)
        {
            return Instance("TestContext", clock, new TestScheduler(clock));
        }
    }
}