// <copyright file="CounterSerializationTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Linq;
using App.Metrics.Counter;
using App.Metrics.Middleware.Formatters.Json.Facts.Helpers;
using App.Metrics.Middleware.Formatters.Json.Facts.TestFixtures;
using App.Metrics.Middleware.Formatters.Json.Serialization;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace App.Metrics.Middleware.Formatters.Json.Facts
{
    public class CounterSerializationTests : IClassFixture<MetricProviderTestFixture>
    {
        private readonly CounterValueSource _counter;
        private readonly ITestOutputHelper _output;
        private readonly MetricDataSerializer _serializer;

        public CounterSerializationTests(ITestOutputHelper output, MetricProviderTestFixture fixture)
        {
            _output = output;
            _serializer = new MetricDataSerializer();
            _counter = fixture.Counters.First();

            _counter = fixture.Counters.First(x => x.Name == fixture.CounterNameDefault);
        }

        [Fact]
        public void Can_deserialize()
        {
            var jsonCounter = MetricType.Counter.SampleJson();

            var result = _serializer.Deserialize<CounterValueSource>(jsonCounter.ToString());

            result.Name.Should().BeEquivalentTo(_counter.Name);
            result.Unit.Should().Be(_counter.Unit);
            result.Value.Count.Should().Be(_counter.Value.Count);
            result.Value.Items.Should().BeEquivalentTo(_counter.Value.Items);

            result.Tags.Keys.Should().Contain(_counter.Tags.Keys.ToArray());
            result.Tags.Values.Should().Contain(_counter.Tags.Values.ToArray());
        }

        [Fact]
        public void Produces_expected_json()
        {
            var expected = MetricType.Counter.SampleJson();

            var result = _serializer.Serialize(_counter).ParseAsJson();

            result.Should().Be(expected);
        }

        [Fact]
        public void Produces_valid_Json()
        {
            var json = _serializer.Serialize(_counter);
            _output.WriteLine("Json Counter: {0}", json);

            Action action = () => JToken.Parse(json);
            action.ShouldNotThrow<Exception>();
        }
    }
}