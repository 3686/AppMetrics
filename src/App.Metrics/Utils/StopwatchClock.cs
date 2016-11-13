﻿using System;
using System.Diagnostics;
using System.Globalization;

namespace App.Metrics.Utils
{
    public sealed class StopwatchClock : IClock
    {
        private static readonly long Factor = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

        public long Nanoseconds => Stopwatch.GetTimestamp() * Factor;

        public DateTime UtcDateTime => DateTime.UtcNow;

#pragma warning disable 67
        public event EventHandler Advanced;
#pragma warning restore 67

        public long Seconds => TimeUnit.Nanoseconds.ToSeconds(Nanoseconds);

        public void Advance(TimeUnit unit, long value)
        {
            throw new NotImplementedException($"Unable to advance {GetType()} Clock Type");
        }

        public string FormatTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("yyyy-MM-ddTHH:mm:ss.ffffK", CultureInfo.InvariantCulture);
        }
    }
}