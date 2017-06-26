﻿// <copyright file="CustomHealthCheckTestStartup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.AspNetCore.Middleware.Options;
using App.Metrics.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Metrics.AspNetCore.Integration.Facts.Startup
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
                DefaultContextLabel = "testing",
                MetricsEnabled = true
            };

            var appMetricsMiddlewareOptions = new AppMetricsMiddlewareOptions
            {
                MetricsTextEndpointEnabled = true,
                HealthEndpointEnabled = true,
                MetricsEndpointEnabled = true,
                PingEndpointEnabled = true,
                HealthEndpoint = new PathString("/health-status")
            };

            SetupServices(services, appMetricsOptions, appMetricsMiddlewareOptions);
        }
    }
}