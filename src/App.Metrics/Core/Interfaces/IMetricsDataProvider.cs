﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using App.Metrics.Data;

namespace App.Metrics.Core.Interfaces
{
    /// <summary>
    ///     Provides access to the current metrics recorded by the application
    /// </summary>
    public interface IMetricsDataProvider
    {
        /// <summary>
        ///     Retrieves a snapshot of the current metrics values recorded.
        /// </summary>
        /// <param name="context">The metric context to retreive.</param>
        /// <returns>Metrics data belonging to the specified context</returns>
        MetricsContextValueSource ReadContext(string context);

        /// <summary>
        ///     Returns the current metrics data for the context for which this provider has been created.
        /// </summary>
        /// <returns>
        ///     A snapshot of the current metrics data, if a global <see cref="IMetricsFilter" /> is configured this will be
        ///     applied on the result.
        /// </returns>
        MetricsDataValueSource ReadData();

        /// <summary>
        ///     Returns the current metrics data for the context for which this provider has been created.
        /// </summary>
        /// <param name="overrideGlobalFilter">
        ///     The override the configured global filter and filters metric data by the specified
        ///     <see cref="IMetricsFilter" />.
        /// </param>
        /// <returns>
        ///     A snapshot of the current metrics data filtered by the specified <see cref="IMetricsFilter" />
        /// </returns>
        MetricsDataValueSource ReadData(IMetricsFilter overrideGlobalFilter);
    }
}