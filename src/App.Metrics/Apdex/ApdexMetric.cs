﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.Core;
using App.Metrics.Core.Abstractions;

namespace App.Metrics.Apdex
{
    public sealed class ApdexMetric : MetricBase
    {
        public int Frustrating { get; set; }

        public int SampleSize { get; set; }

        public int Satisfied { get; set; }

        public double Score { get; set; }

        public int Tolerating { get; set; }
    }
}