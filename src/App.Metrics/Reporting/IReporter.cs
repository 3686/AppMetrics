// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.Reporting
{
    public interface IReporter
    {
        Task RunReports(IMetrics context, CancellationToken token);
    }
}