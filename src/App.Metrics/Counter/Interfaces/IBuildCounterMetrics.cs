﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Counter.Interfaces;

namespace App.Metrics.Counter.Interfaces
{
    public interface IBuildCounterMetrics
    {
        ICounterMetric Build();

        ICounterMetric Build<T>(Func<T> builder)
            where T : ICounterMetric;
    }
}