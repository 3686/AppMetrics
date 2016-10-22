﻿using System;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics.DataProviders;
using App.Metrics.Health;
using App.Metrics.Internal;
using App.Metrics.Registries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace App.Metrics.Facts.Health
{
    public class HealthCheckRegistryTests
    {
        private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
        private readonly IHealthCheckManager _healthCheckManager;
        private readonly IHealthCheckRegistry _healthCheckRegistry;

        public HealthCheckRegistryTests()
        {
            _healthCheckRegistry = new DefaultHealthCheckRegistry(LoggerFactory, Enumerable.Empty<HealthCheck>(),
                Options.Create(new AppMetricsOptions()));
            _healthCheckManager = new DefaultHealthCheckManager(LoggerFactory, _healthCheckRegistry);
        }

        [Fact]
        public void HealthCheck_RegistryDoesNotThrowOnDuplicateRegistration()
        {
            _healthCheckRegistry.UnregisterAllHealthChecks();

            _healthCheckRegistry.Register("test", () => Task.FromResult(HealthCheckResult.Healthy()));

            Action action = () => _healthCheckRegistry.Register("test", () => Task.FromResult(HealthCheckResult.Healthy()));
            action.ShouldNotThrow<InvalidOperationException>();
        }

        [Fact]
        public async Task HealthCheck_RegistryExecutesCheckOnEachGetStatus()
        {
            _healthCheckRegistry.UnregisterAllHealthChecks();
            var count = 0;

            _healthCheckRegistry.Register("test", () =>
            {
                count++;
                return Task.FromResult(HealthCheckResult.Healthy());
            });

            count.Should().Be(0);

            await _healthCheckManager.GetStatusAsync();

            count.Should().Be(1);

            await _healthCheckManager.GetStatusAsync();

            count.Should().Be(2);
        }

        [Fact]
        public async Task HealthCheck_RegistryStatusIsFailedIfOneCheckFails()
        {
            _healthCheckRegistry.UnregisterAllHealthChecks();

            _healthCheckRegistry.Register("ok", () => Task.FromResult(HealthCheckResult.Healthy()));
            _healthCheckRegistry.Register("bad", () => Task.FromResult(HealthCheckResult.Unhealthy()));

            var status = await _healthCheckManager.GetStatusAsync();

            status.IsHealthy.Should().BeFalse();
            status.Results.Length.Should().Be(2);
        }

        [Fact]
        public async Task HealthCheck_RegistryStatusIsHealthyIfAllChecksAreHealthy()
        {
            _healthCheckRegistry.UnregisterAllHealthChecks();

            _healthCheckRegistry.Register("ok", () => Task.FromResult(HealthCheckResult.Healthy()));
            _healthCheckRegistry.Register("another", () => Task.FromResult(HealthCheckResult.Healthy()));

            var status = await _healthCheckManager.GetStatusAsync();

            status.IsHealthy.Should().BeTrue();
            status.Results.Length.Should().Be(2);
        }
    }
}