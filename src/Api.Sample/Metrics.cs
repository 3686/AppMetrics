﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Core;

namespace Api.Sample
{
    public static class Metrics
    {
        public static class Groups
        {
            public static class TestGroup
            {
                public static readonly string TestGroupName = "Test Group";

                public static class Counters
                {
                    public static CounterOptions TestCounter { get; } = new CounterOptions
                    {
                        Name = "Test Counter",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };

                    public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
                    {
                        Name = "Test Counter With Item",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };
                }

                public static class Gauges
                {
                    public static GaugeOptions TestGauge { get; } = new GaugeOptions
                    {
                        Name = "Test Gauge",
                        MeasurementUnit = Unit.Items,
                        Tags = MetricTags.None
                    };
                }

                public static class Histograms
                {
                    public static HistogramOptions TestHistogram { get; } = new HistogramOptions
                    {
                        Name = "Test Histogram",
                        SamplingType = SamplingType.HighDynamicRange,
                        MeasurementUnit = Unit.MegaBytes,
                        Tags = MetricTags.None
                    };

                    public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
                    {
                        Name = "Test Histogram With User Value",
                        SamplingType = SamplingType.HighDynamicRange,
                        MeasurementUnit = Unit.Bytes,
                        Tags = MetricTags.None
                    };
                }

                public static class Meters
                {
                    public static MeterOptions TestMeter { get; } = new MeterOptions
                    {
                        Name = "Test Meter",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };
                }

                public static class Timers
                {
                    public static TimerOptions TestTimer { get; } = new TimerOptions
                    {
                        Name = "Test Timer",
                        MeasurementUnit = Unit.Items,
                        DurationUnit = TimeUnit.Milliseconds,
                        RateUnit = TimeUnit.Milliseconds,
                        Tags = MetricTags.None,
                        SamplingType = SamplingType.ExponentiallyDecaying,
                    };

                    public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
                    {
                        MeasurementUnit = Unit.Items,
                        DurationUnit = TimeUnit.Milliseconds,
                        RateUnit = TimeUnit.Milliseconds,
                        Tags = MetricTags.None,
                        SamplingType = SamplingType.ExponentiallyDecaying,
                    };
                }
            }

            public static class TestGroupTwo
            {
                public static readonly string TestGroupName = "Test Group Two";

                public static class Counters
                {
                    public static CounterOptions TestCounter { get; } = new CounterOptions
                    {
                        Name = "Test Counter",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };

                    public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
                    {
                        Name = "Test Counter With Item",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };
                }

                public static class Gauges
                {
                    public static GaugeOptions TestGauge { get; } = new GaugeOptions
                    {
                        Name = "Test Gauge",
                        MeasurementUnit = Unit.Items,
                        Tags = MetricTags.None
                    };
                }

                public static class Histograms
                {
                    public static HistogramOptions TestHistogram { get; } = new HistogramOptions
                    {
                        Name = "Test Histogram",
                        SamplingType = SamplingType.HighDynamicRange,
                        MeasurementUnit = Unit.MegaBytes,
                        Tags = MetricTags.None
                    };

                    public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
                    {
                        Name = "Test Histogram With User Value",
                        SamplingType = SamplingType.HighDynamicRange,
                        MeasurementUnit = Unit.Bytes,
                        Tags = MetricTags.None
                    };
                }

                public static class Meters
                {
                    public static MeterOptions TestMeter { get; } = new MeterOptions
                    {
                        Name = "Test Meter",
                        MeasurementUnit = Unit.Calls,
                        Tags = MetricTags.None
                    };
                }

                public static class Timers
                {
                    public static TimerOptions TestTimer { get; } = new TimerOptions
                    {
                        Name = "Test Timer",
                        MeasurementUnit = Unit.Items,
                        DurationUnit = TimeUnit.Milliseconds,
                        RateUnit = TimeUnit.Milliseconds,
                        Tags = MetricTags.None,
                        SamplingType = SamplingType.ExponentiallyDecaying,
                    };

                    public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
                    {
                        Name = "Test Timer With User Value",
                        MeasurementUnit = Unit.Items,
                        DurationUnit = TimeUnit.Milliseconds,
                        RateUnit = TimeUnit.Milliseconds,
                        Tags = MetricTags.None,
                        SamplingType = SamplingType.ExponentiallyDecaying,
                    };
                }
            }
        }

        public static class Counters
        {
            public static CounterOptions TestCounter { get; } = new CounterOptions
            {
                Name = "Test Counter",
                MeasurementUnit = Unit.Calls,
                Tags = MetricTags.None
            };

            public static CounterOptions TestCounterWithItem { get; } = new CounterOptions
            {
                Name = "Test Counter With Item",
                MeasurementUnit = Unit.Calls,
                Tags = MetricTags.None
            };
        }

        public static class Gauges
        {
            public static GaugeOptions TestGauge { get; } = new GaugeOptions
            {
                Name = "Test Gauge",
                MeasurementUnit = Unit.Items,
                Tags = MetricTags.None
            };
        }

        public static class Histograms
        {
            public static HistogramOptions TestHistogram { get; } = new HistogramOptions
            {
                Name = "Test Histogram",
                SamplingType = SamplingType.ExponentiallyDecaying,
                MeasurementUnit = Unit.MegaBytes,
                Tags = MetricTags.None
            };

            public static HistogramOptions TestHAdvancedistogram { get; } = new HistogramOptions
            {
                Name = "Test Advanced Histogram",
                SamplingType = SamplingType.ExponentiallyDecaying,
                MeasurementUnit = Unit.MegaBytes,
                Tags = MetricTags.None
            };

            public static HistogramOptions TestHistogramWithUserValue { get; } = new HistogramOptions
            {
                Name = "Test Histogram With User Value",
                SamplingType = SamplingType.ExponentiallyDecaying,
                MeasurementUnit = Unit.Bytes,
                Tags = MetricTags.None
            };
        }

        public static class Meters
        {
            public static MeterOptions TestMeter { get; } = new MeterOptions
            {
                Name = "Test Meter",
                MeasurementUnit = Unit.Calls,
                Tags = MetricTags.None
            };
        }

        public static class Timers
        {
            public static TimerOptions TestTimer { get; } = new TimerOptions
            {
                Name = "Test Timer",
                MeasurementUnit = Unit.Items,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Tags = MetricTags.None,
                SamplingType = SamplingType.ExponentiallyDecaying,
            };

            public static TimerOptions TestTimerWithUserValue { get; } = new TimerOptions
            {
                Name = "Test Timer With User Value",
                MeasurementUnit = Unit.Items,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Tags = MetricTags.None,
                SamplingType = SamplingType.ExponentiallyDecaying,
            };

            public static TimerOptions TestTimerTwo { get; } = new TimerOptions
            {
                Name = "Test Timer 2",
                MeasurementUnit = Unit.Items,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Tags = MetricTags.None,
                SamplingType = SamplingType.ExponentiallyDecaying,
            };

            public static TimerOptions TestTimerTwoWithUserValue { get; } = new TimerOptions
            {
                Name = "Test Timer 2 With User Value",
                MeasurementUnit = Unit.Items,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds,
                Tags = MetricTags.None,
                SamplingType = SamplingType.ExponentiallyDecaying,
            };
        }
    }
}
