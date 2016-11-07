using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Data;
using App.Metrics.Infrastructure;
using App.Metrics.Internal;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Facts.Fixtures
{
    public class DefaultMetricsContextTestFixture : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        public IMetricsContext Context { get; }

        public Func<IMetricsContext, Task<MetricsDataValueSource>> CurrentData => 
            async ctx => await Context.Advanced.DataManager.GetAsync();

        public Func<IMetricsContext, IMetricsFilter, Task<MetricsDataValueSource>> CurrentDataWithFilter
            => async (ctx, filter) => await Context.Advanced.DataManager.WithFilter(filter).GetAsync();

        public DefaultMetricsContextTestFixture()
        {
            var options = Microsoft.Extensions.Options.Options.Create(new AppMetricsOptions());
            var healthCheckManager = new DefaultHealthCheckManager(_loggerFactory, () => new ConcurrentDictionary<string, HealthCheck>());
            var metricsBuilder = new DefaultMetricsBuilder(options.Value.Clock);
            Func<string, IMetricGroupRegistry> newMetricsGroupRegistry = name => new DefaultMetricGroupRegistry(name);
            var registry = new DefaultMetricsRegistry(_loggerFactory, options, new EnvironmentInfoBuilder(_loggerFactory), newMetricsGroupRegistry);
            var metricsDataManager = new DefaultMetricsDataManager(registry);
            Context = new DefaultMetricsContext(options, registry, metricsBuilder, healthCheckManager, metricsDataManager);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Context?.Advanced.DataManager.Reset();
            Context?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}