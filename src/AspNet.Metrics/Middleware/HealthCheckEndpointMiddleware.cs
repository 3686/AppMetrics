// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Metrics.Middleware
{
    public class HealthCheckEndpointMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private readonly HealthStatusSerializer _serializer;

        public HealthCheckEndpointMiddleware(RequestDelegate next,
            IOptions<AspNetMetricsOptions> options,
            ILoggerFactory loggerFactory,
            IMetricsContext metricsContext)
            : base(next, options, loggerFactory, metricsContext)
        {
            _serializer = new HealthStatusSerializer(MetricsContext.Advanced.Clock);
        }

        public async Task Invoke(HttpContext context)
        {
            if (Options.HealthEndpointEnabled &&
                Options.HealthEndpoint.HasValue &&
                Options.HealthEndpoint == context.Request.Path)
            {
                Logger.MiddlewareExecuting(GetType());

                var healthStatus = await MetricsContext.Advanced.HealthCheckManager.GetStatusAsync();
                var responseStatusCode = healthStatus.IsHealthy ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;

                var json = _serializer.Serialize(healthStatus);

                await Task.FromResult(WriteResponseAsync(context, json, "application/json", responseStatusCode));

                Logger.MiddlewareExecuted(GetType());

                return;
            }

            await Next(context);
        }
    }
}