﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace App.Metrics.Reporting
{
    public class AppMetricsReportingOptions
    {
        public AppMetricsReportingOptions()
        {
            IsEnabled = true;
            Reporters = factory => { };
        }

        public bool IsEnabled { get; set; }

        public Action<IReportFactory> Reporters { get; set; }
    }
}