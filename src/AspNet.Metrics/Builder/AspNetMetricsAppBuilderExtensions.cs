using System;
using App.Metrics;
using App.Metrics.Internal;
using AspNet.Metrics;
using AspNet.Metrics.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace
namespace Microsoft.AspNet.Builder
// ReSharper restore CheckNamespace
{
    public static class AspNetMetricsAppBuilderExtensions
    {
        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // Verify if AddMetrics was done before calling UseMetrics
            // We use the MetricsMarkerService to make sure if all the services were added.
            MetricsServicesHelper.ThrowIfMetricsNotRegistered(app.ApplicationServices);

            var appMetricsOptions = app.ApplicationServices.GetRequiredService<IOptions<AppMetricsOptions>>().Value;
            var aspNetMetricsOptions = app.ApplicationServices.GetRequiredService<IOptions<AspNetMetricsOptions>>().Value;


            app.UseMiddleware<PingEndpointMiddleware>();


            if (aspNetMetricsOptions.HealthEnabled && !appMetricsOptions.DisableHealthChecks)
            {
                app.UseMiddleware<HealthCheckEndpointMiddleware>();
            }

            if (aspNetMetricsOptions.MetricsTextEnabled && !appMetricsOptions.DisableMetrics)
            {
                app.UseMiddleware<MetricsEndpointTextEndpointMiddleware>(appMetricsOptions.MetricsFilter);
            }

            if (aspNetMetricsOptions.MetricsEnabled && !appMetricsOptions.DisableMetrics)
            {
                app.UseMiddleware<MetricsEndpointMiddleware>(appMetricsOptions.MetricsFilter);
            }

            if (!appMetricsOptions.DisableMetrics)
            {
                app.UseMiddleware<ActiveRequestCounterEndpointMiddleware>();
                app.UseMiddleware<ErrorRequestMeterMiddleware>();
                app.UseMiddleware<OAuth2ClientWebRequestMeterMiddleware>();
                app.UseMiddleware<PerRequestTimerMiddleware>();
                app.UseMiddleware<PostAndPutRequestSizeHistogramMiddleware>();
                app.UseMiddleware<RequestTimerMiddleware>();
            }


            return app;
        }
    }
}