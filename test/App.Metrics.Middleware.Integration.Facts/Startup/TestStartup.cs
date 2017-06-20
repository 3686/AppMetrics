// <copyright file="TestStartup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Metrics.Builder;
using App.Metrics.Core;
using App.Metrics.Core.Configuration;
using App.Metrics.Core.Infrastructure;
using App.Metrics.Core.ReservoirSampling.Uniform;
using App.Metrics.Filters;
using App.Metrics.Health;
using App.Metrics.Middleware.DependencyInjection.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware.Integration.Facts.Startup
{
    public abstract class TestStartup
    {
        protected IMetrics Metrics { get; private set; }

        protected void SetupAppBuilder(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMetrics();

            app.Use(
                (context, func) =>
                {
                    var clientId = string.Empty;

                    if (context.Request.Path.Value.Contains("oauth"))
                    {
                        clientId = context.Request.Path.Value.Split('/').Last();
                    }

                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        context.User =
                            new ClaimsPrincipal(
                                new List<ClaimsIdentity>
                                {
                                    new ClaimsIdentity(
                                        new[]
                                        {
                                            new Claim("client_id", clientId)
                                        })
                                });
                    }

                    return func();
                });

            Metrics = app.ApplicationServices.GetRequiredService<IMetrics>();

            app.UseMvc();
        }

        protected void SetupServices(
            IServiceCollection services,
            AppMetricsOptions appMetricsOptions,
            AppMetricsMiddlewareOptions appMetricsMiddlewareOptions,
            IFilterMetrics filter = null,
            IEnumerable<HealthCheckResult> healthChecks = null)
        {
            services
                .AddLogging()
                .AddRouting(options => { options.LowercaseUrls = true; });

            services.AddMvc(options => options.AddMetricsResourceFilter());

            var builder = services
                .AddMetrics(
                    options =>
                    {
                        options.DefaultContextLabel = appMetricsOptions.DefaultContextLabel;
                        options.MetricsEnabled = appMetricsOptions.MetricsEnabled;
                    })
                .AddDefaultReservoir(() => new DefaultAlgorithmRReservoir(1028))
                .AddClockType<TestClock>()
                .AddHealthChecks(
                    factory =>
                    {
                        var checks = healthChecks != null
                            ? healthChecks.ToList()
                            : new List<HealthCheckResult>();

                        for (var i = 0; i < checks.Count; i++)
                        {
                            var check = checks[i];
                            factory.Register("Check" + i, () => Task.FromResult(check));
                        }
                    })
                .AddMetricsMiddleware(
                    options =>
                    {
                        options.MetricsTextEndpointEnabled = appMetricsMiddlewareOptions.MetricsTextEndpointEnabled;
                        options.HealthEndpointEnabled = appMetricsMiddlewareOptions.HealthEndpointEnabled;
                        options.MetricsEndpointEnabled = appMetricsMiddlewareOptions.MetricsEndpointEnabled;
                        options.PingEndpointEnabled = appMetricsMiddlewareOptions.PingEndpointEnabled;
                        options.OAuth2TrackingEnabled = appMetricsMiddlewareOptions.OAuth2TrackingEnabled;

                        options.HealthEndpoint = appMetricsMiddlewareOptions.HealthEndpoint;
                        options.MetricsEndpoint = appMetricsMiddlewareOptions.MetricsEndpoint;
                        options.MetricsTextEndpoint = appMetricsMiddlewareOptions.MetricsTextEndpoint;
                        options.PingEndpoint = appMetricsMiddlewareOptions.PingEndpoint;

                        options.IgnoredRoutesRegexPatterns = appMetricsMiddlewareOptions.IgnoredRoutesRegexPatterns;
                        options.IgnoredHttpStatusCodes = appMetricsMiddlewareOptions.IgnoredHttpStatusCodes;

                        options.DefaultTrackingEnabled = appMetricsMiddlewareOptions.DefaultTrackingEnabled;
                    },
                    optionsBuilder =>
                    {
                        optionsBuilder.AddJsonMetricsSerialization().AddAsciiMetricsTextSerialization();
                    });

            if (filter != null)
            {
                builder.AddGlobalFilter(filter);
            }
        }
    }
}