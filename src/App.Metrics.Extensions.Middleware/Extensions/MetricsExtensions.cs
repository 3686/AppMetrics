﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Extensions.Middleware.Internal;
using App.Metrics.Gauge;

// ReSharper disable CheckNamespace
namespace App.Metrics
{
    // ReSharper restore CheckNamespace
    internal static class MetricsExtensions
    {
        public static IMetrics DecrementActiveRequests(this IMetrics metrics)
        {
            metrics.Measure.Counter.Decrement(AspNetMetricsRegistry.Contexts.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        public static IMetrics ErrorRequestPercentage(this IMetrics metrics)
        {
            var errors = metrics.Provider.Meter.Instance(AspNetMetricsRegistry.Contexts.HttpRequests.Meters.HttpErrorRequests);
            var requests = metrics.Provider.Timer.Instance(AspNetMetricsRegistry.Contexts.HttpRequests.Timers.WebRequestTimer);

            metrics.Measure.Gauge.SetValue(
                AspNetMetricsRegistry.Contexts.HttpRequests.Gauges.PercentageErrorRequests,
                () => new HitPercentageGauge(errors, requests, m => m.OneMinuteRate));

            return metrics;
        }

        public static IMetrics IncrementActiveRequests(this IMetrics metrics)
        {
            metrics.Measure.Counter.Increment(AspNetMetricsRegistry.Contexts.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        public static IMetrics MarkHttpEndpointForOAuthClient(this IMetrics metrics, string routeTemplate, string clientId, int httpStatusCode)
        {
            metrics.Measure.Meter.Mark(
                AspNetMetricsRegistry.Contexts.OAuth2.Meters.EndpointHttpRequests(routeTemplate),
                item => item.With("client_id", clientId).With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestEndpointError(this IMetrics metrics, string routeTemplate, int httpStatusCode)
        {
            metrics.Measure.Meter.Mark(
                AspNetMetricsRegistry.Contexts.HttpRequests.Meters.EndpointHttpErrorRequests(routeTemplate),
                item => item.With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestError(this IMetrics metrics, int httpStatusCode)
        {
            metrics.Measure.Meter.Mark(
                AspNetMetricsRegistry.Contexts.HttpRequests.Meters.HttpErrorRequests,
                item => item.With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics MarkHttpRequestForOAuthClient(this IMetrics metrics, string clientId, int httpStatusCode)
        {
            metrics.Measure.Meter.Mark(
                AspNetMetricsRegistry.Contexts.OAuth2.Meters.HttpRequests,
                item => item.With("client_id", clientId).With("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        public static IMetrics RecordEndpointRequestTime(this IMetrics metrics, string clientId, string routeTemplate, long elapsed)
        {
            metrics.Provider
                   .Timer
                   .Instance(AspNetMetricsRegistry.Contexts.HttpRequests.Timers.EndpointPerRequestTimer(routeTemplate))
                   .Record(elapsed, TimeUnit.Nanoseconds, clientId.IsPresent() ? clientId : null);

            return metrics;
        }

        public static IMetrics UpdatePostAndPutRequestSize(this IMetrics metrics, long value)
        {
            metrics.Measure.Histogram.Update(AspNetMetricsRegistry.Contexts.HttpRequests.Histograms.PostAndPutRequestSize, value);

            return metrics;
        }
    }
}