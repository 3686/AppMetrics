﻿using System;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics.Health;
using App.Metrics.Registries;

namespace App.Metrics.DataProviders
{
    public sealed class DefaultHealthCheckDataProvider : IHealthCheckDataProvider
    {
        private readonly IHealthCheckRegistry _healthCheckRegistry;

        public DefaultHealthCheckDataProvider(IHealthCheckRegistry healthCheckRegistry)
        {
            if (healthCheckRegistry == null)
            {
                throw new ArgumentNullException(nameof(healthCheckRegistry));
            }

            _healthCheckRegistry = healthCheckRegistry;
        }

        public async Task<HealthStatus> GetStatusAsync()
        {
            var results = await Task.WhenAll(_healthCheckRegistry.Checks.Values.OrderBy(v => v.Name).Select(v => v.ExecuteAsync()));

            return new HealthStatus(results);
        }
    }
}