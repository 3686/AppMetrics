﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


 // ReSharper disable CheckNamespace
namespace App.Metrics.Core
// ReSharper restore CheckNamespace
{
    public static class HealthCheckStatusExtensions
    {
        public static bool IsHealthy(this HealthCheckStatus status)
        {
            return status == HealthCheckStatus.Healthy;
        }

        public static bool IsUnhealthy(this HealthCheckStatus status)
        {
            return status == HealthCheckStatus.Unhealthy;
        }

        public static bool IsDegraded(this HealthCheckStatus status)
        {
            return status == HealthCheckStatus.Degraded;
        }

        public static bool IsIgnored(this HealthCheckStatus status)
        {
            return status == HealthCheckStatus.Ignored;
        }
    }
}