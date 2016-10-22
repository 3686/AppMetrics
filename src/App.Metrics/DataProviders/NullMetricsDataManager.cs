// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.MetricData;

namespace App.Metrics.DataProviders
{
    public sealed class NullMetricsDataManager : IMetricsDataManager
    {
        public MetricsData GetMetricsData()
        {
            return MetricsData.Empty;
        }
    }
}