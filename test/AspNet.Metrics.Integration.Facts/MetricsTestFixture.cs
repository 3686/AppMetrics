﻿using System;
using System.Net.Http;
using App.Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.Metrics.Integration.Facts
{
    public class MetricsHostTestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;

        public MetricsHostTestFixture()
        {
            var builder = new WebHostBuilder().UseStartup<TStartup>();
            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Context = _server.Host.Services.GetRequiredService<IMetrics>();
        }

        public HttpClient Client { get; }

        public IMetrics Context { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}