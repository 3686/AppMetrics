﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using App.Metrics.Core.Interfaces;
using App.Metrics.Data;
using App.Metrics.Interfaces;

namespace App.Metrics.Internal.Managers
{
    internal class DefaultMetricValuesProvider : IProvideMetricValues
    {
        private readonly IFilterMetrics _globalFilter;
        private readonly IMetricsRegistry _registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultMetricValuesProvider" /> class.
        /// </summary>
        /// <param name="globalFilter">The global filter.</param>
        /// <param name="registry">The registry.</param>
        public DefaultMetricValuesProvider(IFilterMetrics globalFilter, IMetricsRegistry registry)
        {
            _globalFilter = globalFilter ?? new NoOpMetricsFilter();
            _registry = registry ?? new NullMetricsRegistry();
        }

        /// <inheritdoc />
        public MetricsContextValueSource GetForContext(string context)
        {
            var data = Get();

            var filter = new DefaultMetricsFilter().WhereContext(context);

            var contextData = data.Filter(filter);

            return contextData.Contexts.Single();
        }

        /// <inheritdoc />
        public MetricsDataValueSource Get() { return _registry.GetData(_globalFilter); }

        /// <inheritdoc />
        public MetricsDataValueSource Get(IFilterMetrics overrideGlobalFilter) { return _registry.GetData(overrideGlobalFilter); }
    }
}