﻿using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Filtering;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Sandbox.JustForTesting;
using App.Metrics.Tagging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Sandbox
{
    public enum ReportType
    {
        InfluxDB,
        ElasticSearch,
        Graphite
    }

    public class Startup
    {
        private static readonly string ElasticSearchIndex = "appmetricssandbox";
        private static readonly Uri ElasticSearchUri = new Uri("http://127.0.0.1:9200");
        private static readonly Uri GraphiteUri = new Uri("net.tcp://127.0.0.1:32776");
        private static readonly bool HaveAppRunSampleRequests = true;
        private static readonly string InfluxDbDatabase = "AppMetricsSandbox";
        private static readonly Uri InfluxDbUri = new Uri("http://127.0.0.1:8086");

        private static readonly List<ReportType> ReportTypes =
            new List<ReportType> { ReportType.InfluxDB/*, ReportType.ElasticSearch, ReportType.Graphite*/ };

        private static readonly bool RunSamplesWithClientId = true;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).
                                                     AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).
                                                     AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true).
                                                     AddEnvironmentVariables();

            Configuration = builder.Build();
            Env = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Env { get; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            if (RunSamplesWithClientId && HaveAppRunSampleRequests)
            {
                app.Use(
                    (context, func) =>
                    {
                        RandomClientIdForTesting.SetTheFakeClaimsPrincipal(context);
                        return func();
                    });
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();

            app.UseMetrics();
            app.UseMetricsReporting(lifetime);

            app.UseMvc();

            if (HaveAppRunSampleRequests)
            {
                SampleRequests.Run(lifetime.ApplicationStopping);
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTestStuff();
            services.AddLogging().AddRouting(options => { options.LowercaseUrls = true; });

            services.AddMvc(options => options.AddMetricsResourceFilter());

            var reportFilter = new DefaultMetricsFilter();
            reportFilter.WithHealthChecks(false);

            services.AddMetrics(Configuration.GetSection("AppMetrics")).                     
                     AddJsonMetricsSerialization().
                     AddAsciiHealthSerialization().
                     AddAsciiMetricsTextSerialization().
                      AddReporting(
                         factory =>
                         {
                             if (ReportTypes.Any(r => r == ReportType.InfluxDB))
                             {
                                 factory.AddInfluxDb(
                                     new InfluxDBReporterSettings
                                     {
                                         HttpPolicy = new HttpPolicy
                                         {
                                             FailuresBeforeBackoff = 3,
                                             BackoffPeriod = TimeSpan.FromSeconds(30),
                                             Timeout = TimeSpan.FromSeconds(10)
                                         },
                                         InfluxDbSettings = new InfluxDBSettings(InfluxDbDatabase, InfluxDbUri),
                                         ReportInterval = TimeSpan.FromSeconds(5)
                                     },
                                     reportFilter);
                             }}).

                             //        if (ReportTypes.Any(r => r == ReportType.ElasticSearch))
                             //        {
                             //            factory.AddElasticSearch(
                             //                new ElasticSearchReporterSettings
                             //                {
                             //                    HttpPolicy = new Extensions.Reporting.ElasticSearch.HttpPolicy
                             //                                 {
                             //                                     FailuresBeforeBackoff = 3,
                             //                                     BackoffPeriod = TimeSpan.FromSeconds(30),
                             //                                     Timeout = TimeSpan.FromSeconds(10)
                             //                                 },
                             //                    ElasticSearchSettings = new ElasticSearchSettings(ElasticSearchUri, ElasticSearchIndex),
                             //                    ReportInterval = TimeSpan.FromSeconds(5)
                             //                },
                             //                reportFilter);
                             //        }

                             //        if (ReportTypes.Any(r => r == ReportType.ElasticSearch))
                             //        {
                             //            factory.AddGraphite(
                             //                new GraphiteReporterSettings
                             //                {
                             //                    HttpPolicy = new Extensions.Reporting.Graphite.HttpPolicy
                             //                                 {
                             //                                     FailuresBeforeBackoff = 3,
                             //                                     BackoffPeriod = TimeSpan.FromSeconds(30),
                             //                                     Timeout = TimeSpan.FromSeconds(3)
                             //                                 },
                             //                    GraphiteSettings = new GraphiteSettings(GraphiteUri),
                             //                    ReportInterval = TimeSpan.FromSeconds(5)
                             //                });
                             //        }
                             //    }).
                             AddHealthChecks(
                         factory =>
                         {
                             factory.RegisterPingHealthCheck("google ping", "google.com", TimeSpan.FromSeconds(10));

                             factory.RegisterHttpGetHealthCheck("github", new Uri("https://github.com/"), TimeSpan.FromSeconds(10));
                             
                             factory.RegisterMetricCheck(
                                 name: "Database Call Duration",
                                 options: SandboxMetricsRegistry.DatabaseTimer,
                                 tags: new MetricTags("client_id", "client-9"),
                                 passing: value => (message: $"OK. 98th Percentile < 100ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 < 100),
                                 warning: value => (message: $"WARNING. 98th Percentile > 100ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 < 200),
                                 failing: value => (message: $"FAILED. 98th Percentile > 200ms ({value.Histogram.Percentile98}{SandboxMetricsRegistry.DatabaseTimer.DurationUnit.Unit()})", result: value.Histogram.Percentile98 > 200));

                         }).
                     AddMetricsMiddleware();
        }
    }
}