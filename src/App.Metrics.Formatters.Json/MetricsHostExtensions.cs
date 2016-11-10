﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using App.Metrics.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Json
{
    public static class MetricsHostExtensions
    {
        public static IMetricsHostBuilder AddJsonSerialization(this IMetricsHostBuilder host)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricDataSerializer, MetricDataSerializer>());
            host.Services.Replace(ServiceDescriptor.Transient<IHealthStatusSerializer, HealthStatusSerializer>());

            return host;
        }

        public static IMetricsHostBuilder AddJsonSerialization(this IMetricsHostBuilder host, JsonSerializerSettings serializerSettings)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricDataSerializer>(provider => new MetricDataSerializer(serializerSettings)));
            host.Services.Replace(ServiceDescriptor.Transient<IHealthStatusSerializer>(provider => new HealthStatusSerializer(serializerSettings)));

            return host;
        }
    }
}