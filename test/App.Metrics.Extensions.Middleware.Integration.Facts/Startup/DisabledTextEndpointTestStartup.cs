﻿// <copyright file="DisabledTextEndpointTestStartup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Configuration;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware.Integration.Facts.Startup
{
    public class DisabledTextEndpointTestStartup : TestStartup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupAppBuilder(app, env, loggerFactory);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appMetricsOptions = new AppMetricsOptions();

            var appMetricsMiddlewareOptions = new AppMetricsMiddlewareOptions
                                       {
                                           MetricsTextEndpointEnabled = false
                                       };

            SetupServices(services, appMetricsOptions, appMetricsMiddlewareOptions);
        }
    }
}