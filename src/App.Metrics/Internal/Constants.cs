﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using App.Metrics.Core;

namespace App.Metrics.Internal
{
    internal static class Constants
    {
        public static class Health
        {
            public const string DegradedStatusDisplay = "Degraded";
            public const string HealthyStatusDisplay = "Healthy";
            public const string UnhealthyStatusDisplay = "Unhealthy";

            public static ReadOnlyDictionary<HealthCheckStatus, string> HealthStatusDisplay =
                new ReadOnlyDictionary<HealthCheckStatus, string>(new Dictionary<HealthCheckStatus, string>
                {
                    { HealthCheckStatus.Healthy, HealthyStatusDisplay },
                    { HealthCheckStatus.Unhealthy, UnhealthyStatusDisplay },
                    { HealthCheckStatus.Degraded, DegradedStatusDisplay }
                });
        }

        public static class ReservoirSampling
        {
            public const int DefaultSampleSize = 1028;
            public const double DefaultExponentialDecayFactor = 0.015;
            public const double DefaultApdexTSeconds = 0.5;
        }
    }
}