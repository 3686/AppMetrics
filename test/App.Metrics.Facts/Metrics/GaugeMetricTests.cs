﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using App.Metrics.Gauge;
using App.Metrics.Infrastructure;
using App.Metrics.Internal;
using App.Metrics.Meter;
using App.Metrics.ReservoirSampling;
using App.Metrics.ReservoirSampling.Uniform;
using App.Metrics.Tagging;
using App.Metrics.Timer;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Facts.Metrics
{
    public class GaugeMetricTests
    {
        [Theory]
        [InlineData(2.0, 4.0, 50.0)]
        [InlineData(0.0, 4.0, 0.0)]
        [InlineData(4.0, 2.0, 100.0)]
        [InlineData(4.0, 0.0, 100.0)]
        public void can_calculate_percentage(double numerator, double denominator, double expectedPercentage)
        {
            var hitPercentage = new PercentageGauge(() => numerator, () => denominator);

            hitPercentage.Value.Should().Be(expectedPercentage);
        }

        [Fact]
        public void can_calculate_the_hit_ratio_as_a_guage()
        {
            var clock = new TestClock();
            var scheduler = new TestTaskScheduler(clock);
            var reservoir = new Lazy<IReservoir>(() => new DefaultAlgorithmRReservoir(1028));

            var cacheHitMeter = new DefaultMeterMetric(clock, scheduler);
            var dbQueryTimer = new DefaultTimerMetric(reservoir, clock);

            foreach (var index in Enumerable.Range(0, 1000))
            {
                using (dbQueryTimer.NewContext())
                {
                    clock.Advance(TimeUnit.Milliseconds, 100);
                }

                if (index % 2 == 0)
                {
                    cacheHitMeter.Mark();
                }
            }

            var cacheHitRatioGauge = new HitRatioGauge(cacheHitMeter, dbQueryTimer, value => value.OneMinuteRate);

            cacheHitRatioGauge.Value.Should().BeGreaterThan(0.0);
        }

        [Fact]
        public void can_calculate_the_hit_ratio_as_a_guage_with_one_min_rate_as_default()
        {
            var clock = new TestClock();
            var scheduler = new TestTaskScheduler(clock);
            var reservoir = new Lazy<IReservoir>(() => new DefaultAlgorithmRReservoir(1028));

            var cacheHitMeter = new DefaultMeterMetric(clock, scheduler);
            var dbQueryTimer = new DefaultTimerMetric(reservoir, clock);

            foreach (var index in Enumerable.Range(0, 1000))
            {
                using (dbQueryTimer.NewContext())
                {
                    clock.Advance(TimeUnit.Milliseconds, 100);
                }

                if (index % 2 == 0)
                {
                    cacheHitMeter.Mark();
                }
            }

            var cacheHitRatioGauge = new HitRatioGauge(cacheHitMeter, dbQueryTimer);

            cacheHitRatioGauge.Value.Should().BeGreaterThan(0.0);
        }

        [Fact]
        public void can_create_gauge_from_value_source()
        {
            var valueSource = new GaugeValueSource("test", new FunctionGauge(() => 2.0), Unit.Bytes, MetricTags.None);

            var gauge = GaugeMetric.FromGauge(valueSource);

            gauge.Value.Should().Be(2.0);
            gauge.Name.Should().Be("test");
            gauge.Tags.Count.Should().Be(0);
            Assert.True(gauge.Unit == Unit.Bytes);
        }

        [Fact]
        public void should_report_nan_on_exception()
        {
            new FunctionGauge(() => { throw new InvalidOperationException("test"); }).Value.Should().Be(double.NaN);

            new DerivedGauge(new FunctionGauge(() => 5.0), (d) => { throw new InvalidOperationException("test"); }).Value.Should().Be(double.NaN);
        }
    }
}