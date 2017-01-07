// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
#if NET452
using System.Reflection;
#endif
using App.Metrics;
using Microsoft.Extensions.PlatformAbstractions;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    internal sealed class MetricsHostBuilder : IMetricsHostBuilder
    {
#if NET452
        internal MetricsHostBuilder(IServiceCollection services, AssemblyName assemblyName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            Services = services;
            Environment = new MetricsAppEnvironment(PlatformServices.Default.Application, assemblyName);
        }
#endif

#if !NET452
        internal MetricsHostBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
            Environment = new MetricsAppEnvironment(PlatformServices.Default.Application);
        }
#endif

        public IMetricsEnvironment Environment { get; }

        public IServiceCollection Services { get; }
    }
}