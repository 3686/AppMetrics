﻿using System;
using System.Reflection;
using App.Metrics.Core;
using App.Metrics.Sampling;
using Xunit;
using FluentAssertions;

namespace App.Metrics.Facts.Sampling
{
    public class DefaultSamplingTypeTests
    {
        private static readonly FieldInfo reservoirField = typeof(HistogramMetric).GetField("reservoir", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Reservoir GetReservoir(HistogramMetric histogram)
        {
            return reservoirField.GetValue(histogram) as Reservoir;
        }

        [Fact]
        public void SamplingType_CanUseConfiguredDefaultSamplingType()
        {
            GetReservoir(new HistogramMetric()).Should().BeOfType<ExponentiallyDecayingReservoir>();

            Metric.Config.WithDefaultSamplingType(SamplingType.HighDynamicRange);

            GetReservoir(new HistogramMetric()).Should().BeOfType<HdrHistogramReservoir>();

            Metric.Config.WithDefaultSamplingType(SamplingType.LongTerm);

            GetReservoir(new HistogramMetric()).Should().BeOfType<UniformReservoir>();
        }

        [Fact]
        public void SamplingType_SettingDefaultValueMustBeConcreteValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Metric.Config.WithDefaultSamplingType(SamplingType.Default);
            });
        }
    }
}
