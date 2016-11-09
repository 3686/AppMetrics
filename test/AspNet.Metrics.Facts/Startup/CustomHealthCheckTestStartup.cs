﻿using App.Metrics;
using App.Metrics.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNet.Metrics.Facts.Startup
{
    public class CustomHealthCheckTestStartup : TestStartup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupAppBuilder(app, env, loggerFactory);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appMetricsOptions = new AppMetricsOptions
            {
                DefaultGroupName = "testing",
                DisableMetrics = false,
                Clock = new Clock.TestClock(),
                DefaultSamplingType = SamplingType.LongTerm
            };

            var aspNetMetricsOptions = new AspNetMetricsOptions
            {
                MetricsTextEndpointEnabled = true,
                HealthEndpointEnabled = true,
                MetricsEndpointEnabled = true,
                PingEndpointEnabled = true,
                HealthEndpoint = new PathString("/health-status")
            };

            SetupServices(services, appMetricsOptions, aspNetMetricsOptions);
        }
    }
}