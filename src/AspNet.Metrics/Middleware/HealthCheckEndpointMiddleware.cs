using System.Net;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.DataProviders;
using App.Metrics.Json;
using App.Metrics.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Metrics.Middleware
{
    public class HealthCheckEndpointMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private readonly IHealthCheckDataProvider _healthCheckDataProvider;
        private readonly IClock _systemClock;

        public HealthCheckEndpointMiddleware(RequestDelegate next,
            IOptions<AspNetMetricsOptions> options,
            IClock systemClock,
            ILoggerFactory loggerFactory,
            IMetricsContext metricsContext,
            IHealthCheckDataProvider healthCheckDataProvider)
            : base(next, options, loggerFactory, metricsContext)
        {
            _systemClock = systemClock;
            _healthCheckDataProvider = healthCheckDataProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (Options.HealthEnabled && Options.HealthEndpoint.HasValue && Options.HealthEndpoint == context.Request.Path)
            {
                var healthStatus = await _healthCheckDataProvider.GetStatusAsync();
                var responseStatusCode = healthStatus.IsHealthy ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                await
                    Task.FromResult(WriteResponseAsync(context, JsonHealthChecks.BuildJson(healthStatus, _systemClock, true), "application/json",
                        responseStatusCode));
                return;
            }

            await Next(context);
        }
    }
}