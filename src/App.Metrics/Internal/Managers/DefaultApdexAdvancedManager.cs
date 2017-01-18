﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Core.Options;
using App.Metrics.Interfaces;
using App.Metrics.Utils;

namespace App.Metrics.Internal.Managers
{
    public class DefaultApdexAdvancedManager : IMeasureApdexMetricsAdvanced
    {
        private readonly IBuildApdexMetrics _apdexBuidler;
        private readonly IClock _clock;
        private readonly IMetricsRegistry _registry;

        public DefaultApdexAdvancedManager(
            IBuildApdexMetrics apdexBuidler,
            IMetricsRegistry registry,
            IClock clock)
        {
            _registry = registry;
            _clock = clock;
            _apdexBuidler = apdexBuidler;
        }

        /// <inheritdoc />
        public IApdex With(ApdexOptions options)
        {
            if (options.WithReservoir != null)
            {
                return With(options, () => _apdexBuidler.Instance(options.WithReservoir(), options.ApdexTSeconds, options.AllowWarmup, _clock));
            }

            return _registry.Apdex(
                options,
                () =>
                    _apdexBuidler.Instance(
                        options.SamplingType,
                        options.SampleSize,
                        options.ExponentialDecayFactor,
                        options.ApdexTSeconds,
                        options.AllowWarmup,
                        _clock));
        }

        /// <inheritdoc />
        public IApdex With<T>(ApdexOptions options, Func<T> builder)
            where T : IApdexMetric
        {
            return _registry.Apdex(options, builder);
        }
    }
}