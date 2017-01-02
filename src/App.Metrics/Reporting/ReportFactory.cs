﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Reporting.Internal;
using App.Metrics.Scheduling;
using App.Metrics.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Reporting
{
    public sealed class ReportFactory : IReportFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<Type, IReporterProvider> _providers = new Dictionary<Type, IReporterProvider>();
        private readonly object _syncLock = new object();

        public ReportFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void AddProvider(IReporterProvider provider)
        {
            lock (_syncLock)
            {
                _providers.Add(provider.GetType(), provider);
            }
        }

        public IReporter CreateReporter(IScheduler scheduler)
        {
            return new Reporter(this, scheduler, _loggerFactory);
        }

        public IReporter CreateReporter()
        {
            return CreateReporter(new DefaultTaskScheduler());
        }

        internal Dictionary<Type, IReporterProvider> GetProviders()
        {
            return _providers;
        }
    }
}