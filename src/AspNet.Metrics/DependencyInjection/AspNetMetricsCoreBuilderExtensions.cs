// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using AspNet.Metrics;
using AspNet.Metrics.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    public static class AspNetMetricsCoreBuilderExtensions
    {
        public static IMetricsHostBuilder AddAspNetMetrics(
            this IMetricsHostBuilder host)
        {
            host.AddAspNetMetrics(setupAction: null);
            return host;
        }

        public static IMetricsHostBuilder AddAspNetMetrics(
            this IMetricsHostBuilder host,
            Action<AspNetMetricsOptions> setupAction)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            ConfigureDefaultServices(host.Services);

            AddAspNetMetricsCoreServices(host.Services);

            if (setupAction != null)
            {
                host.Services.Configure(setupAction);
            }

            return host;
        }

        internal static void AddAspNetMetricsCoreServices(IServiceCollection services)
        {
            services.TryAddSingleton<AspNetMetricsMarkerService, AspNetMetricsMarkerService>();
            services.TryAddSingleton(provider => provider.GetRequiredService<IOptions<AspNetMetricsOptions>>().Value);

            //TODO: AH - remove this or add setup config here?
            //services.TryAddEnumerable(
            //    ServiceDescriptor.Transient<IConfigureOptions<AspNetMetricsOptions>, AspNetMetricsOptionsSetup>());
        }

        private static void ConfigureDefaultServices(IServiceCollection services)
        {
            services.AddRouting();
        }
    }
}