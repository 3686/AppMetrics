﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using App.Metrics.Core.Options;
using App.Metrics.Facts.Fixtures;
using App.Metrics.Interfaces;
using App.Metrics.Internal;
using App.Metrics.Internal.Managers;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Facts.Managers
{
    public class DefaultMeterManagerTests : IClassFixture<MetricManagerTestFixture>
    {
        private readonly MetricManagerTestFixture _fixture;
        private readonly IMeasureMeterMetrics _manager;

        public DefaultMeterManagerTests(MetricManagerTestFixture fixture)
        {
            _fixture = fixture;
            _manager = new DefaultMeterManager(_fixture.Builder.Meter, _fixture.Registry, _fixture.Clock);
        }

        [Fact]
        public void can_mark()
        {
            var metricName = "test_mark_meter";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options);

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().Meters.Count(x => x.Name == metricName).Should().Be(1);
        }

        [Fact]
        public void can_mark_by_amount()
        {
            var metricName = "test_mark_meter_by_amount";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options, 2L);

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().Meters.Count(x => x.Name == metricName).Should().Be(1);
        }

        [Fact]
        public void can_mark_with_item()
        {
            var metricName = "test_mark_meter_with_item";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options, "item1");

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().MeterValueFor(metricName).Items.Length.Should().Be(1);
        }

        [Fact]
        public void can_mark_with_item_by_amount()
        {
            var metricName = "test_mark_meter_with_item_by_amount";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options, 5L, "item1");

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().MeterValueFor(metricName).Items.Length.Should().Be(1);
        }

        [Fact]
        public void can_mark_with_metric_item()
        {
            var metricName = "test_mark_meter_with_metric_item";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options, item => { item.With("tagKey", "tagvalue"); });

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().MeterValueFor(metricName).Items.Length.Should().Be(1);
        }

        [Fact]
        public void can_mark_with_metric_item_by_amount()
        {
            var metricName = "test_mark_meter_with_metric_item_by_amount";
            var options = new MeterOptions { Name = metricName };

            _manager.Mark(options, 5L, item => { item.With("tagKey", "tagvalue"); });

            var data = _fixture.Registry.GetData(new NoOpMetricsFilter());

            data.Contexts.Single().MeterValueFor(metricName).Items.Length.Should().Be(1);
        }
    }
}