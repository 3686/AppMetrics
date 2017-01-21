﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Core;
using App.Metrics.ReservoirSampling;
using App.Metrics.ReservoirSampling.ExponentialDecay;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Facts.Metrics
{
    public class HistogramMetricTests
    {
        private readonly HistogramMetric _histogram;

        public HistogramMetricTests()
        {
            var reservoir = new Lazy<IReservoir>(() => new DefaultForwardDecayingReservoir());
            _histogram = new HistogramMetric(reservoir);
        }

        [Fact]
        public void can_count()
        {
            _histogram.Update(1L);
            _histogram.Value.Count.Should().Be(1);
            _histogram.Update(1L);
            _histogram.Value.Count.Should().Be(2);
        }

        [Fact]
        public void can_reset()
        {
            _histogram.Update(1L);
            _histogram.Update(10L);

            _histogram.Value.Count.Should().NotBe(0);
            _histogram.Value.LastValue.Should().NotBe(0);
            _histogram.Value.Median.Should().NotBe(0);

            _histogram.Reset();

            _histogram.Value.Count.Should().Be(0);
            _histogram.Value.LastValue.Should().Be(0);
            _histogram.Value.Median.Should().Be(0);
        }

        [Fact]
        public void records_mean_for_one_element()
        {
            _histogram.Update(1L);
            _histogram.Value.Mean.Should().Be(1);
        }

        [Fact]
        public void records_user_value()
        {
            _histogram.Update(1L, "A");
            _histogram.Update(10L, "B");

            _histogram.Value.MinUserValue.Should().Be("A");
            _histogram.Value.MaxUserValue.Should().Be("B");
        }
    }
}