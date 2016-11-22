// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Serialization.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using App.Metrics.Core;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware.Middleware
{
    public class HealthCheckEndpointMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private readonly IHealthStatusSerializer _serializer;

        public HealthCheckEndpointMiddleware(RequestDelegate next,
            AspNetMetricsOptions aspNetOptions,
            ILoggerFactory loggerFactory,
            IMetrics metrics,
            IHealthStatusSerializer serializer)
            : base(next, aspNetOptions, loggerFactory, metrics)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            _serializer = serializer;
        }

        public async Task Invoke(HttpContext context)
        {
            if (Options.HealthEndpointEnabled &&
                Options.HealthEndpoint.IsPresent() &&
                Options.HealthEndpoint == context.Request.Path)
            {
                Logger.MiddlewareExecuting(GetType());

                var healthStatus = await Metrics.Advanced.Health.ReadStatusAsync();
                string warning = null;

                var responseStatusCode = HttpStatusCode.OK;

                if (healthStatus.Status.IsUnhealthy())
                {
                    responseStatusCode = HttpStatusCode.InternalServerError;
                }

                if (healthStatus.Status.IsDegraded())
                {
                    responseStatusCode = HttpStatusCode.OK;
                    warning = App.Metrics.Internal.Constants.Health.DegradedStatusDisplay;
                }

                var json = _serializer.Serialize(healthStatus);

                await WriteResponseAsync(context, json, "application/json", responseStatusCode, warning);

                Logger.MiddlewareExecuted(GetType());

                return;
            }

            await Next(context);
        }
    }
}