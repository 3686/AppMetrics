using System;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Extensions.Middleware.Internal;

// ReSharper disable CheckNamespace
namespace App.Metrics
// ReSharper restore CheckNamespace
{
    public class ApdexHealthCheck : HealthCheck
    {
        private readonly Lazy<IMetrics> _metrics;
        private readonly AspNetMetricsOptions _options;

        public ApdexHealthCheck(Lazy<IMetrics> metrics, AspNetMetricsOptions options)
            : base("Apdex Score")
        {
            _metrics = metrics;
            _options = options;
        }

        protected override async Task<HealthCheckResult> CheckAsync()
        {
            if (!_options.ApdexTrackingEnabled)
            {
                return HealthCheckResult.Ignore();
            }

            var metricsContext = await _metrics.Value.Advanced.Data.ReadContextAsync(AspNetMetricsRegistry.Contexts.HttpRequests.ContextName);

            var apdex = metricsContext.ApdexValueFor(AspNetMetricsRegistry.Contexts.HttpRequests.ApdexScores.ApdexMetricName);

            if (apdex.Score < 0.5)
            {
                return HealthCheckResult.Unhealthy($"Frustrating. Score: {apdex.Score}");
            }

            if (apdex.Score >= 0.5 && apdex.Score < 0.75)
            {
                return HealthCheckResult.Degraded($"Tolerating. Score: {apdex.Score}");
            }

            return HealthCheckResult.Healthy($"Satisfied. Score {apdex.Score}");
        }
    }
}