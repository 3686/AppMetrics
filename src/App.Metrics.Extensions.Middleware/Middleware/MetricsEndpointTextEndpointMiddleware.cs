﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware.Middleware
{
    public class MetricsEndpointTextEndpointMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
    {
        private readonly DefaultReportGenerator _reportGenerator;

        public MetricsEndpointTextEndpointMiddleware(
            RequestDelegate next,
            AspNetMetricsOptions aspNetOptions,
            ILoggerFactory loggerFactory,
            IMetrics metrics)
            : base(next, aspNetOptions, loggerFactory, metrics)
        {
            _reportGenerator = new DefaultReportGenerator(loggerFactory);
        }

        public async Task Invoke(HttpContext context)
        {
            if (Options.MetricsTextEndpointEnabled && Options.MetricsTextEndpoint.IsPresent() && Options.MetricsTextEndpoint == context.Request.Path)
            {
                Logger.MiddlewareExecuting(GetType());

                var stringReporter = new StringReporter();
                await _reportGenerator.GenerateAsync(stringReporter, Metrics, context.RequestAborted);

                await WriteResponseAsync(context, stringReporter.Result, "text/plain");

                Logger.MiddlewareExecuted(GetType());

                return;
            }

            await Next(context);
        }
    }
}