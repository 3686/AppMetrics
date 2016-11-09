﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using AspNet.Metrics.Facts.Startup;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AspNet.Metrics.Facts.Middleware
{
    public class HealthCheckEndpointMiddlewareTests : IClassFixture<MetricsHostTestFixture<DefaultTestStartup>>
    {
        public HealthCheckEndpointMiddlewareTests(MetricsHostTestFixture<DefaultTestStartup> fixture)
        {
            Client = fixture.Client;
            Context = fixture.Context;
        }

        public HttpClient Client { get; }

        public IMetricsContext Context { get; }

        [Fact]
        public async Task can_count_errors_per_endpoint_and_also_get_a_total_error_count()
        {
            var result = await Client.GetAsync("/health");

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(Skip = "Requires too man test startup classes, how can options be passed to IClassTestFixture?")]
        public async Task can_disable_health_checks()
        {
            //var fixture = new MetricsTestFixture(new AppMetricsOptions
            //{
            //    DefaultGroupName = "testing",
            //    DisableMetrics = false,
            //    DefaultSamplingType = SamplingType.LongTerm
            //}, enableHealthChecks:false, testAspNetOptions:new AspNetMetricsOptions {HealthEndpointEnabled = false});

            //var result = await fixture.Client.GetAsync("/health");

            //result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(Skip = "Requires too man test startup classes, how can options be passed to IClassTestFixture?")]
        public async Task can_disable_health_check_endpoint_but_leave_health_checks_tracking_enabled_for_reporting()
        {
            //var fixture = new MetricsTestFixture(new AppMetricsOptions
            //{
            //    DefaultGroupName = "testing",
            //    DisableMetrics = false,
            //    DefaultSamplingType = SamplingType.LongTerm
            //}, enableHealthChecks: true, testAspNetOptions: new AspNetMetricsOptions { HealthEndpointEnabled = false });

            //var result = await fixture.Client.GetAsync("/health");

            //result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(Skip = "Requires too man test startup classes, how can options be passed to IClassTestFixture?")]
        public async Task can_disable_health_endpoint_when_health_enabled()
        {
            //var fixture = new MetricsTestFixture(new AppMetricsOptions
            //{
            //    DefaultGroupName = "testing",
            //    DisableMetrics = false,
            //    DefaultSamplingType = SamplingType.LongTerm
            //}, new AspNetMetricsOptions { HealthEndpointEnabled = false });

            //var result = await fixture.Client.GetAsync("/health");

            //result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(Skip = "Requires too man test startup classes, how can options be passed to IClassTestFixture?")]
        public async Task can_change_health_endpoint()
        {
            //var fixture = new MetricsTestFixture(new AppMetricsOptions
            //{
            //    DefaultGroupName = "testing",
            //    DisableMetrics = false,
            //    DefaultSamplingType = SamplingType.LongTerm
            //}, new AspNetMetricsOptions {HealthEndpoint = new PathString("/health-check")});

            //var result = await fixture.Client.GetAsync("/health");
            //result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            //result = await fixture.Client.GetAsync("/health-check");
            //result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}