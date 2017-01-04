﻿using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Facts.Fixtures;
using App.Metrics.Facts.Reporting.Helpers;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Reporting.Internal;
using App.Metrics.Scheduling;
using App.Metrics.Scheduling.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace App.Metrics.Facts.Reporting
{
    public class ReporterTests : IClassFixture<MetricsReportingFixture>
    {
        private readonly MetricsReportingFixture _fixture;

        public ReporterTests(MetricsReportingFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void schedules_reports_to_run_at_the_specified_interval()
        {
            var interval = TimeSpan.FromSeconds(60);
            var loggerFactory = new LoggerFactory();
            var metrics = new Mock<IMetrics>();
            var factory = new ReportFactory(metrics.Object, loggerFactory);
            var provider = new Mock<IReporterProvider>();
            var scheduler = new Mock<IScheduler>();
            var metricReporter = new Mock<IMetricReporter>();
            metricReporter.Setup(r => r.GetType()).Returns(typeof(IMetricReporter));
            var settings = new Mock<IReporterSettings>();
            settings.Setup(s => s.ReportInterval).Returns(interval);
            metricReporter.Setup(r => r.ReportInterval).Returns(interval);
            provider.Setup(p => p.CreateMetricReporter(It.IsAny<string>(), It.IsAny<ILoggerFactory>())).Returns(metricReporter.Object);
            factory.AddProvider(provider.Object);

            var reporter = new Reporter(factory, metrics.Object, scheduler.Object, loggerFactory);

            reporter.RunReports(_fixture.Metrics, CancellationToken.None);

            scheduler.Verify(p => p.Interval(interval, TaskCreationOptions.LongRunning, It.IsAny<Action>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void when_metric_report_fails_should_not_throw()
        {
            var loggerFactory = new LoggerFactory();
            var metrics = new Mock<IMetrics>();
            var factory = new ReportFactory(metrics.Object, loggerFactory);
            factory.AddProvider(new TestReportProvider(false, TimeSpan.FromMilliseconds(10), new Exception()));
            var scheduler = new DefaultTaskScheduler();
            var reporter = new Reporter(factory, metrics.Object, scheduler, loggerFactory);
            var token = new CancellationTokenSource();
            token.CancelAfter(100);

            Action action = () => { reporter.RunReports(_fixture.Metrics, token.Token); };

            action.ShouldNotThrow();
        }

        [Fact]
        public void when_null_providers_doest_throw()
        {
            var loggerFactory = new LoggerFactory();
            var metrics = new Mock<IMetrics>();
            var factory = new ReportFactory(metrics.Object, loggerFactory);
            var scheduler = new Mock<IScheduler>();
            var reporter = new Reporter(factory, metrics.Object, scheduler.Object, loggerFactory);

            reporter.RunReports(_fixture.Metrics, CancellationToken.None);
        }

        [Fact]
        public void when_provider_added_the_associated_metric_reporter_is_created()
        {
            var loggerFactory = new LoggerFactory();
            var metrics = new Mock<IMetrics>();
            var factory = new ReportFactory(metrics.Object, loggerFactory);
            var provider = new Mock<IReporterProvider>();
            var scheduler = new Mock<IScheduler>();
            var metricReporter = new Mock<IMetricReporter>();
            metricReporter.Setup(r => r.GetType()).Returns(typeof(IMetricReporter));
            provider.Setup(p => p.CreateMetricReporter(It.IsAny<string>(), It.IsAny<ILoggerFactory>())).Returns(metricReporter.Object);
            factory.AddProvider(provider.Object);

            var reporter = new Reporter(factory, metrics.Object, scheduler.Object, loggerFactory);

            provider.Verify(p => p.CreateMetricReporter(It.IsAny<string>(), It.IsAny<ILoggerFactory>()), Times.Once);

            reporter.RunReports(_fixture.Metrics, CancellationToken.None);
        }
    }
}