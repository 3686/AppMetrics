// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Net;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNet.Metrics.Middleware
{
    public class PerRequestTimerMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private const string TimerItemsKey = "__Mertics.PerRequestStartTime__";

        public PerRequestTimerMiddleware(RequestDelegate next,
            IOptions<AspNetMetricsOptions> options,
            ILoggerFactory loggerFactory,
            IMetricsContext metricsContext)
            : base(next, options, loggerFactory, metricsContext)
        {
        }

        public async Task Invoke(HttpContext context)
        {
            if (PerformMetric(context))
            {
                Logger.MiddlewareExecuting(GetType());

                context.Items[TimerItemsKey] = MetricsContext.Advanced.Clock.Nanoseconds;

                await Next(context);

                if (context.HasMetricsCurrentRouteName() && context.Response.StatusCode != (int)HttpStatusCode.NotFound)
                {
                    var clientId = context.OAuthClientId();

                    var startTime = (long)context.Items[TimerItemsKey];
                    var elapsed = MetricsContext.Advanced.Clock.Nanoseconds - startTime;

                    MetricsContext.GetWebApplicationContext().Advanced
                        .Timer(context.GetMetricsCurrentRouteName(), Unit.Requests)
                        .Record(elapsed, TimeUnit.Nanoseconds, clientId.IsPresent() ? clientId : null);
                }

                Logger.MiddlewareExecuted(GetType());
            }
            else
            {
                await Next(context);
            }
        }
    }
}