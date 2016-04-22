using System;
using AspNet.Metrics;
using AspNet.Metrics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.OptionsModel;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    public static class MetricsCoreServiceCollectionExtensions
    {
        public static IMetricsBuilder AddMetricsCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return AddMetricsCore(services, setupAction: null);
        }

        public static IMetricsBuilder AddMetricsCore(
            this IServiceCollection services,
            Action<MetricsOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //ConfigureDefaultServices(services);

            AddMetricsCoreServices(services);

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new MetricsBuilder(services);
        }

        // To enable unit testing
        internal static void AddMetricsCoreServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MetricsOptions>, MetricsCoreOptionsSetup>());

            services.TryAddSingleton<MetricsMarkerService, MetricsMarkerService>();
        }

        //private static void ConfigureDefaultServices(IServiceCollection services)
        //{
        //    services.AddRouting();
        //    services.AddOptions();
        //}
    }
}