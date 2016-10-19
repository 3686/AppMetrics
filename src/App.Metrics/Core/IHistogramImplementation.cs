﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy


using System;
using App.Metrics.MetricData;

namespace App.Metrics.Core
{
    public interface IHistogramImplementation : IHistogram, IMetricValueProvider<HistogramValue>, IDisposable
    {
    }
}