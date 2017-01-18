﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Interfaces
{
    public interface IMeasureMetrics
    {
        IMeasureApdexMetrics Apdex { get; }

        IMeasureCounterMetrics Counter { get; }

        IMeasureGaugeMetrics Gauge { get; }

        IMeasureHistogramMetrics Histogram { get; }

        IMeasureMeterMetrics Meter { get; }

        IMeasureTimerMetrics Timer { get; }
    }
}