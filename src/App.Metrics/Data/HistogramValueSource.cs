﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.Data.Interfaces;

namespace App.Metrics.Data
{
    /// <summary>
    ///     Combines the value of the histogram with the defined unit for the value.
    /// </summary>
    public sealed class HistogramValueSource : MetricValueSource<HistogramValue>
    {
        public HistogramValueSource(string name, IMetricValueProvider<HistogramValue> valueProvider, Unit unit, MetricTags tags)
            : base(name, valueProvider, unit, tags) { }
    }
}