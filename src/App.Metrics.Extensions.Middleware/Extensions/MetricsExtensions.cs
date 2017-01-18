﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Core;
using App.Metrics.Extensions.Middleware.Internal;

// ReSharper disable CheckNamespace
namespace App.Metrics
{
    // ReSharper restore CheckNamespace
    internal static class MetricsExtensions
    {
        public static IMetrics DecrementActiveRequests(this IMetrics metrics)
        {
            metrics.Counter.Decrement(AspNetMetricsRegistry.Contexts.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        public static IMetrics ErrorRequestPercentage(this IMetrics metrics)
        {
            var errors = metrics.AdvancedMetrics.Meter.With(AspNetMetricsRegistry.Contexts.HttpRequests.Meters.HttpErrorRequests);
            var requests = metrics.AdvancedMetrics.Timer.With(AspNetMetricsRegistry.Contexts.HttpRequests.Timers.WebRequestTimer);

            metrics.Gauge.SetValue(
                AspNetMetricsRegistry.Contexts.HttpRequests.Gauges.PercentageErrorRequests,
                () => new HitPercentageGauge(errors, requests, m => m.OneMinuteRate));

            return metrics;
        }

        public static IMetrics IncrementActiveRequests(this IMetrics metrics)
        {
            metrics.Counter.Increment(AspNetMetricsRegistry.Contexts.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        public static IMetrics MarkHttpEndpointForOAuthClient(this IMetrics metrics, string routeTemplate, string clientId, int httpStatusCode)
        {
            metrics.Meter.Mark(
                AspNetMetricsRegistry.Contexts.OAuth2.Meters.EndpointHttpRequests(routeTemplate),
                item => item.With("client_id", clientId).With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestEndpointError(this IMetrics metrics, string routeTemplate, int httpStatusCode)
        {
            metrics.Meter.Mark(
                AspNetMetricsRegistry.Contexts.HttpRequests.Meters.EndpointHttpErrorRequests(routeTemplate),
                item => item.With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestError(this IMetrics metrics, int httpStatusCode)
        {
            metrics.Meter.Mark(
                AspNetMetricsRegistry.Contexts.HttpRequests.Meters.HttpErrorRequests,
                item => item.With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestForOAuthClient(this IMetrics metrics, string clientId, int httpStatusCode)
        {
            metrics.Meter.Mark(
                AspNetMetricsRegistry.Contexts.OAuth2.Meters.HttpRequests,
                item => item.With("client_id", clientId).With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics RecordEndpointRequestTime(this IMetrics metrics, string clientId, string routeTemplate, long elapsed)
        {
            metrics.AdvancedMetrics
                   .Timer
                   .With(AspNetMetricsRegistry.Contexts.HttpRequests.Timers.EndpointPerRequestTimer(routeTemplate))
                   .Record(elapsed, TimeUnit.Nanoseconds, clientId.IsPresent() ? clientId : null);

            return metrics;
        }

        public static IMetrics UpdatePostAndPutRequestSize(this IMetrics metrics, long value)
        {
            metrics.Histogram.Update(AspNetMetricsRegistry.Contexts.HttpRequests.Histograms.PostAndPutRequestSize, value);

            return metrics;
        }
    }
}