﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.DependencyInjection;
using App.Metrics.Internal;
using App.Metrics.MetricData;
using App.Metrics.Reporting;
using App.Metrics.Scheduling;
using HealthCheck.Samples;
using Metrics.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;

namespace App.Sample
{
    public class Host
    {
        public static void Main()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ConfigureMetrics(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            var application = new Application(provider);
            var scheduler = new DefaultTaskScheduler();

            var simpleMetrics = new SampleMetrics(application.MetricsContext);
            var setCounterSample = new SetCounterSample(application.MetricsContext);
            var setMeterSample = new SetMeterSample(application.MetricsContext);
            var userValueHistogramSample = new UserValueHistogramSample(application.MetricsContext);
            var userValueTimerSample = new UserValueTimerSample(application.MetricsContext);

            var cancellationTokenSource = new CancellationTokenSource();
            var task = scheduler.Interval(
                TimeSpan.FromMilliseconds(300), () =>
                {
                    setCounterSample.RunSomeRequests();
                    setMeterSample.RunSomeRequests();
                    userValueHistogramSample.RunSomeRequests();
                    //userValueTimerSample.RunSomeRequests(); //TODO: AH - why's this taking so long?
                    simpleMetrics.RunSomeRequests();

                    application.MetricsContext.Gauge(AppMetricsRegistry.Gauges.Errors, () => 1);
                    application.MetricsContext.Gauge(AppMetricsRegistry.Gauges.PercentGauge, () => 1);
                    application.MetricsContext.Gauge(AppMetricsRegistry.Gauges.ApmGauge, () => 1);
                    application.MetricsContext.Gauge(AppMetricsRegistry.Gauges.ParenthesisGauge, () => 1);
                    application.MetricsContext.Gauge(AppMetricsRegistry.Gauges.GaugeWithNoValue, () => double.NaN);
                }, cancellationTokenSource.Token);

            application.Reporter.RunReports(application.MetricsContext, cancellationTokenSource.Token);

            Console.WriteLine("Setup Complete, waiting for report run...");

            Console.ReadKey();
        }

        private static void ConfigureMetrics(IServiceCollection services)
        {
            services
                .AddMetrics(options => { })
                .AddHealthChecks(options =>
                {
                    options.IsEnabled = true;
                    options.HealthChecks = factory =>
                    {
                        factory.Register("DatabaseConnected", () => Task.FromResult("Database Connection OK"));
                        factory.Register("DiskSpace", () =>
                        {
                            var freeDiskSpace = GetFreeDiskSpace();

                            return Task.FromResult(freeDiskSpace <= 512
                                ? HealthCheckResult.Unhealthy("Not enough disk space: {0}", freeDiskSpace)
                                : HealthCheckResult.Unhealthy("Disk space ok: {0}", freeDiskSpace));
                        });
                    };
                })
                .AddReporting(options =>
                {
                    var globalTags = new MetricTags().With("env", "stage");

                    options.Reporters = factory =>
                    {
                        var consoleSettings = new ConsoleReporterSettings
                        {
                            ReportInterval = TimeSpan.FromSeconds(5),
                            GlobalTags = globalTags,
                            Filter = new DefaultMetricsFilter()
                                .WhereType(MetricType.Counter)
                                .WhereTaggedWith("filter-tag1", "filter-tag2")
                                .WithHealthChecks(false)
                                .WithEnvironmentInfo(false)
                        };
                        factory.AddConsole(consoleSettings);

                        var textFileSettings = new TextFileReporterSettings
                        {
                            ReportInterval = TimeSpan.FromSeconds(30),
                            FileName = @"C:\metrics\sample.txt"
                        };

                        factory.AddTextFile(textFileSettings);
                    };
                });
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var env = PlatformServices.Default.Application;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine($@"C:\logs\{env.ApplicationName}", "log-{Date}.txt"))
                .CreateLogger();

            services.AddSingleton<ILoggerFactory>(provider =>
            {
                var logFactory = new LoggerFactory();
                logFactory.AddConsole((l, s) => s == LogLevel.Trace);
                logFactory.AddSerilog(Log.Logger);
                return logFactory;
            });

            services.AddTransient<IDatabase, Database>();
        }

        private static int GetFreeDiskSpace()
        {
            return 1024;
        }
    }

    public class Application
    {
        public Application(IServiceProvider provider)
        {
            MetricsContext = provider.GetRequiredService<IMetricsContext>();

            var reporterFactory = provider.GetRequiredService<IReportFactory>();
            Reporter = reporterFactory.CreateReporter();

            Token = new CancellationToken();
        }

        public IMetricsContext MetricsContext { get; set; }

        public IReporter Reporter { get; set; }

        public CancellationToken Token { get; set; }
    }
}