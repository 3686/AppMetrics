﻿using System.Threading.Tasks;
using App.Metrics.Health;

namespace App.Metrics.Infrastructure
{
    public static class AppMetricsTaskCache
    { 
        public static readonly Task<HealthCheckResult> CompletedHealthyTask = Task.FromResult(HealthCheckResult.Healthy());

        public static readonly Task<HealthCheckResult> CompletedUnHealthyTask = Task.FromResult(HealthCheckResult.Unhealthy());
    }
}