using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using App.Metrics;
using App.Metrics.Core;
using App.Metrics.DataProviders;
using App.Metrics.Infrastructure;
using App.Metrics.Internal;
using App.Metrics.Json;
using App.Metrics.Registries;
using App.Metrics.Reporters;
using App.Metrics.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection.Extensions
// ReSharper restore CheckNamespace
{
    internal static class MetricsCoreServiceCollectionExtensions
    {
        private static readonly IReadOnlyDictionary<JsonSchemeVersion, Type> MetricsJsonBuilderVersionMapping =
            new ReadOnlyDictionary<JsonSchemeVersion, Type>(new Dictionary<JsonSchemeVersion, Type>
            {
                { JsonSchemeVersion.AlwaysLatest, typeof(MetricsJsonBuilderV1) },
                { JsonSchemeVersion.Version1, typeof(MetricsJsonBuilderV1) }
            });

        internal static IMetricsHost AddMetricsCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return AddMetricsCore(services, setupAction: null, metricsContext: default(IMetricsContext));
        }

        internal static IMetricsHost AddMetricsCore(
            this IServiceCollection services,
            Action<AppMetricsOptions> setupAction,
            IMetricsContext metricsContext)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var metricsEnvironment = new MetricsAppEnvironment(PlatformServices.Default.Application);

            services.TryAddSingleton<MetricsMarkerService, MetricsMarkerService>();

            services.ConfigureDefaultServices();
            services.AddDefaultHealthCheckServices(metricsEnvironment);
            services.AddDefaultReporterServices();
            services.AddDefaultJsonServices();
            services.AddMetricsCoreServices(metricsEnvironment, metricsContext);            

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new MetricsHost(services, metricsEnvironment);
        }

        internal static void AddDefaultHealthCheckServices(this IServiceCollection services,
            IMetricsEnvironment environment)
        {
            services.TryAddSingleton<IHealthCheckRegistry, HealthCheckRegistry>();
            services.TryAddSingleton<IHealthCheckDataProvider, DefaultHealthCheckDataProvider>();

            services.AddHealthChecks(environment);
        }

        internal static void AddDefaultReporterServices(this IServiceCollection services)
        {
            services.TryAddSingleton<StringReport, StringReport>();
            services.TryAddSingleton(typeof(IMetricReporterRegistry), provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var context = provider.GetRequiredService<IMetricsContext>();                
                var healthCheckDataProvider = provider.GetRequiredService<IHealthCheckDataProvider>();
                var options = provider.GetRequiredService<IOptions<AppMetricsOptions>>();

                var registry = new MetricReporterRegistry(
                   loggerFactory,
                   context.Advanced.MetricsDataProvider,
                   options.Value.SystemClock,
                   healthCheckDataProvider);

                options.Value.Reporters(registry);

                return registry;
            });
        }

        internal static void AddDefaultJsonServices(this IServiceCollection services)
        {
            services.TryAddSingleton<MetricsJsonBuilderV1, MetricsJsonBuilderV1>();
            services.TryAddSingleton(typeof(IMetricsJsonBuilder), provider =>
            {
                var options = provider.GetRequiredService<IOptions<AppMetricsOptions>>();
                var jsonBuilderType = MetricsJsonBuilderVersionMapping[options.Value.JsonSchemeVersion];
                return provider.GetRequiredService(jsonBuilderType);
            });
        }

        internal static void AddMetricsCoreServices(this IServiceCollection services,
            IMetricsEnvironment environment, IMetricsContext metricsContext)
        {
            services.TryAddTransient<IMetricsRegistry, DefaultMetricsRegistry>();
            services.TryAddSingleton<IMetricsBuilder, DefaultMetricsBuilder>();
            services.TryAddSingleton(typeof(IClock), provider => provider.GetRequiredService<IOptions<AppMetricsOptions>>().Value.SystemClock);
            services.TryAddSingleton<EnvironmentInfoBuilder, EnvironmentInfoBuilder>();

            services.TryAddSingleton(typeof(IMetricsContext), provider =>
            {
                var options = provider.GetRequiredService<IOptions<AppMetricsOptions>>();
                var healthCheckRegistry = provider.GetRequiredService<IHealthCheckRegistry>();
                var healthCheckDataProvider = provider.GetRequiredService<IHealthCheckDataProvider>();
                var metricsBuilder = provider.GetRequiredService<IMetricsBuilder>();

                if (!options.Value.DisableHealthChecks)
                {
                    options.Value.HealthCheckRegistry(healthCheckRegistry);
                }

                if (options.Value.EnableInternalMetrics)
                {
                    //TODO: Review enableing internal metrics
                    //var internalMetricsContexxt = new MetricsContext(BaseMetricsContext.InternalMetricsContextName, options.Value.Clock);
                    //options.Value.MetricsContext.Advanced.AttachContext(BaseMetricsContext.InternalMetricsContextName,
                    //    internalMetricsContexxt);
                }

                if (metricsContext == default(IMetricsContext))
                {
                    metricsContext = new MetricsContext(options.Value.GlobalContextName, options.Value.SystemClock,
                        provider.GetRequiredService<IMetricsRegistry>, metricsBuilder, healthCheckDataProvider);
                }


                if (options.Value.DisableMetrics)
                {
                    metricsContext.Advanced.CompletelyDisableMetrics();
                }

                return metricsContext;
            });

            services.TryAddSingleton(provider => environment);
        }

        private static void ConfigureDefaultServices(this IServiceCollection services)
        {            
            services.AddOptions();
        }
    }
}