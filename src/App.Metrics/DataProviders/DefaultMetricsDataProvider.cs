﻿using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.MetricData;

namespace App.Metrics.DataProviders
{
    public class DefaultMetricsDataProvider : IMetricsDataProvider
    {
        private readonly Func<IEnumerable<IMetricsDataProvider>> _childProviders;
        private readonly string _context;
        private readonly IEnumerable<EnvironmentInfoEntry> _environment;
        private readonly IRegistryDataProvider _registryDataProvider;
        private readonly Func<DateTime> _timestampProvider;

        public DefaultMetricsDataProvider(string context,
            Func<DateTime> timestampProvider,
            IRegistryDataProvider registryDataProvider,
            Func<IEnumerable<IMetricsDataProvider>> childProviders)
            : this(context, timestampProvider, Enumerable.Empty<EnvironmentInfoEntry>(), registryDataProvider, childProviders)
        {
        }

        public DefaultMetricsDataProvider(string context,
            Func<DateTime> timestampProvider,
            IEnumerable<EnvironmentInfoEntry> environment,
            IRegistryDataProvider registryDataProvider,
            Func<IEnumerable<IMetricsDataProvider>> childProviders)
        {
            _context = context;
            _timestampProvider = timestampProvider;
            _environment = environment;
            _registryDataProvider = registryDataProvider;
            _childProviders = childProviders;
        }

        public MetricsData CurrentMetricsData
        {
            get
            {
                return new MetricsData(_context, _timestampProvider(),
                    _environment,
                    _registryDataProvider.Gauges.ToArray(),
                    _registryDataProvider.Counters.ToArray(),
                    _registryDataProvider.Meters.ToArray(),
                    _registryDataProvider.Histograms.ToArray(),
                    _registryDataProvider.Timers.ToArray(),
                    _childProviders().Select(p => p.CurrentMetricsData));
            }
        }
    }
}