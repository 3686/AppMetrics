﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET and will retain the same license
// Ported/Refactored to .NET Standard Library by Allan Hardy

using System;
using App.Metrics.Data;
using App.Metrics.Data.Interfaces;

namespace App.Metrics.Core.Interfaces
{
    /// <summary>
    ///     Provides access to a meter metric implementation e.g. <see cref="MeterMetric" />, allows custom meters to be
    ///     implemented
    /// </summary>
    /// <seealso cref="IMeter" />
    /// <seealso cref="IMetricValueProvider{T}" />
    public interface IMeterMetric : IMeter, IMetricValueProvider<MeterValue>, IDisposable
    {
    }
}