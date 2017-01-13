﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#pragma warning disable SA1515
// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET and will retain the same license
// Ported/Refactored to .NET Standard Library by Allan Hardy
#pragma warning disable SA1515

using App.Metrics.Data;
using App.Metrics.Data.Interfaces;

namespace App.Metrics.Core.Interfaces
{
    /// <summary>
    ///     Provides access to a counter metric implementation e.g. <see cref="CounterMetric" />, allows custom counters to be
    ///     implemented
    /// </summary>
    /// <seealso cref="ICounter" />
    /// <seealso cref="IMetricValueProvider{T}" />
    public interface ICounterMetric : ICounter, IMetricValueProvider<CounterValue>
    {
    }
}