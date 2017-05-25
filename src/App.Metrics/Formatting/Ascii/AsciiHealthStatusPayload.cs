﻿// <copyright file="AsciiHealthStatusPayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Core.Internal;
using App.Metrics.Health;

namespace App.Metrics.Formatting.Ascii
{
    public class AsciiHealthStatusPayload
    {
        private readonly List<AsciiHealthCheckResult> _results = new List<AsciiHealthCheckResult>();

        public void Add(AsciiHealthCheckResult result)
        {
            if (result == null)
            {
                return;
            }

            _results.Add(result);
        }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                return;
            }

            var results = _results.ToList();

            var status = GetOverallStatus(results);

            textWriter.Write($"# OVERALL STATUS: {status}");
            textWriter.Write("\n--------------------------------------------------------------\n");

            foreach (var result in results)
            {
                result.Format(textWriter);
                textWriter.Write('\n');
            }
        }

        private static string GetOverallStatus(IReadOnlyCollection<AsciiHealthCheckResult> results)
        {
            var status = Constants.Health.DegradedStatusDisplay;

            var failed = results.Any(c => c.Status == HealthCheckStatus.Unhealthy);
            var degraded = results.Any(c => c.Status == HealthCheckStatus.Degraded);

            if (!degraded && !failed)
            {
                status = Constants.Health.HealthyStatusDisplay;
            }

            if (failed)
            {
                status = Constants.Health.UnhealthyStatusDisplay;
            }

            return status;
        }
    }
}