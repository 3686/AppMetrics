﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using App.Metrics.Core.Interfaces;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Extensions.Middleware.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware.Middleware
{
    public class ApdexMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private const string ApdexItemsKey = "__App.Mertics.Apdex__";
        private readonly IApdex _apdexTracking;

        public ApdexMiddleware(
            RequestDelegate next,
            AspNetMetricsOptions aspNetOptions,
            ILoggerFactory loggerFactory,
            IMetrics metrics)
            : base(next, aspNetOptions, loggerFactory, metrics)
        {
            _apdexTracking = Metrics.Advanced
                                    .Track(AspNetMetricsRegistry.Contexts.HttpRequests.ApdexScores.Apdex(aspNetOptions.ApdexTSeconds));
        }

        public async Task Invoke(HttpContext context)
        {
            if (PerformMetric(context) && Options.ApdexTrackingEnabled)
            {
                Logger.MiddlewareExecuting(GetType());

                context.Items[ApdexItemsKey] = _apdexTracking.NewContext();

                await Next(context);

                var apdex = context.Items[ApdexItemsKey];

                using (apdex as IDisposable)
                {
                }

                context.Items.Remove(ApdexItemsKey);

                Logger.MiddlewareExecuted(GetType());
            }
            else
            {
                await Next(context);
            }
        }
    }
}