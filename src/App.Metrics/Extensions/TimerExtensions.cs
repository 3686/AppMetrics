// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using App.Metrics.Data;

namespace App.Metrics.Extensions
{
    public static class TimerExtensions
    {
        public static IEnumerable<Timer> ToMetric(this IEnumerable<TimerValueSource> source)
        {
            return source.Select(ToMetric);
        }

        public static Timer ToMetric(this TimerValueSource source)
        {
            var histogramData = new Timer.HistogramData
            {
                LastValue = source.Value.Histogram.LastValue,
                LastUserValue = source.Value.Histogram.LastUserValue,
                Max = source.Value.Histogram.Max,
                MaxUserValue = source.Value.Histogram.MaxUserValue,
                Mean = source.Value.Histogram.Mean,
                Min = source.Value.Histogram.Min,
                MinUserValue = source.Value.Histogram.MinUserValue,
                StdDev = source.Value.Histogram.StdDev,
                Median = source.Value.Histogram.Median,
                Percentile75 = source.Value.Histogram.Percentile75,
                Percentile95 = source.Value.Histogram.Percentile95,
                Percentile98 = source.Value.Histogram.Percentile98,
                Percentile99 = source.Value.Histogram.Percentile99,
                Percentile999 = source.Value.Histogram.Percentile999,
                SampleSize = source.Value.Histogram.SampleSize,
            };

            var rateData = new Timer.RateData
            {
                MeanRate = source.Value.Rate.MeanRate,
                OneMinuteRate = source.Value.Rate.OneMinuteRate,
                FiveMinuteRate = source.Value.Rate.FiveMinuteRate,
                FifteenMinuteRate = source.Value.Rate.FifteenMinuteRate
            };

            return new Timer
            {
                Name = source.Name,
                Count = source.Value.Rate.Count,
                ActiveSessions = source.Value.ActiveSessions,
                TotalTime = source.Value.TotalTime,
                Rate = rateData,
                Histogram = histogramData,
                Unit = source.Unit.Name,
                RateUnit = source.RateUnit.Unit(),
                DurationUnit = source.DurationUnit.Unit(),
                Tags = source.Tags.ToDictionary()
            };
        }

        public static TimerValueSource ToMetricValueSource(this Timer source)
        {
            var rateUnit = source.RateUnit.FromUnit();
            var durationUnit = source.DurationUnit.FromUnit();
            var rateValue = new MeterValue(source.Count, source.Rate.MeanRate, source.Rate.OneMinuteRate, source.Rate.FiveMinuteRate,
                source.Rate.FifteenMinuteRate, rateUnit);
            var histogramValue = new HistogramValue(source.Count,
                source.Histogram.LastValue,
                source.Histogram.LastUserValue,
                source.Histogram.Max,
                source.Histogram.MaxUserValue,
                source.Histogram.Mean,
                source.Histogram.Min,
                source.Histogram.MinUserValue,
                source.Histogram.StdDev,
                source.Histogram.Median,
                source.Histogram.Percentile75,
                source.Histogram.Percentile95,
                source.Histogram.Percentile98,
                source.Histogram.Percentile99,
                source.Histogram.Percentile999,
                source.Histogram.SampleSize);

            var timerValue = new TimerValue(rateValue, histogramValue, source.ActiveSessions, source.TotalTime, durationUnit);

            return new TimerValueSource(source.Name, ConstantValue.Provider(timerValue), source.Unit, rateUnit, durationUnit, source.Tags);
        }

        public static IEnumerable<TimerValueSource> ToMetricValueSource(this IEnumerable<Timer> source)
        {
            return source.Select(x => x.ToMetricValueSource());
        }
    }
}