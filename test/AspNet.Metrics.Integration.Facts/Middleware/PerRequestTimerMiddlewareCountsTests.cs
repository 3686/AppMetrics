﻿using System.Net.Http;
using System.Threading.Tasks;
using App.Metrics;
using AspNet.Metrics.Integration.Facts.Startup;
using FluentAssertions;
using Xunit;

namespace AspNet.Metrics.Integration.Facts.Middleware
{
    public class PerRequestTimerMiddlewareCountsTests : IClassFixture<MetricsHostTestFixture<DefaultTestStartup>>
    {
        public PerRequestTimerMiddlewareCountsTests(MetricsHostTestFixture<DefaultTestStartup> fixture)
        {
            Client = fixture.Client;
            Context = fixture.Context;
        }

        public HttpClient Client { get; }

        public IMetrics Context { get; }

        [Fact]
        public async Task can_count_requests_per_endpoint_and_also_get_a_total_count()
        {
            await Client.GetAsync("/api/test");
            await Client.GetAsync("/api/test/error");

            var metrics = await Context.Advanced.Data.ReadContextAsync("Application.WebRequests");

            metrics.TimerValueFor("GET api/test").Histogram.Count.Should().Be(1);
            metrics.TimerValueFor("GET api/test/error").Histogram.Count.Should().Be(1);
            metrics.TimerValueFor("Web Requests").Histogram.Count.Should().Be(2);
        }
    }
}