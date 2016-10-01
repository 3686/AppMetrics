﻿using System.Collections.Generic;
using System.Globalization;
using App.Metrics.MetricData;
using App.Metrics.Utils;

namespace App.Metrics.Json
{
    public static class JsonBuilderV1
    {
        public const string MetricsMimeType = "application/vnd.metrics.net.v1.metrics+json";
        public const int Version = 1;

        private const bool DefaultIndented = true;

        public static string BuildJson(MetricsData data)
        {
            return BuildJson(data, AppEnvironment.Current, Clock.Default, indented: DefaultIndented);
        }

        public static string BuildJson(MetricsData data, IEnumerable<EnvironmentEntry> environment, Clock clock, bool indented = DefaultIndented)
        {
            var version = Version.ToString(CultureInfo.InvariantCulture);

            return JsonMetricsContext.FromContext(data, environment, version)
                .ToJsonObject()
                .AsJson(indented);
        }
    }
}