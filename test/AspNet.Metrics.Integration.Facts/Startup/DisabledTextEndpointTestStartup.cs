﻿using App.Metrics;
using App.Metrics.Configuration;
using App.Metrics.Utils;
using AspNet.Metrics.DependencyInjection.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNet.Metrics.Integration.Facts.Startup
{
    public class DisabledTextEndpointTestStartup : TestStartup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupAppBuilder(app, env, loggerFactory);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appMetricsOptions = new AppMetricsOptions
            {
                DefaultSamplingType = SamplingType.LongTerm
            };

            var aspNetMetricsOptions = new AspNetMetricsOptions
            {
                MetricsTextEndpointEnabled = false
            };

            SetupServices(services, appMetricsOptions, aspNetMetricsOptions);
        }
    }
}