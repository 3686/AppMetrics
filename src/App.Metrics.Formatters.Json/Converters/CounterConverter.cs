﻿using System;
using App.Metrics.Data;
using App.Metrics.Extensions;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Json
{
    public class CounterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(CounterValueSource) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var source = serializer.Deserialize<Counter>(reader);
            return source.ToMetricValueSource();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var source = (CounterValueSource)value;

            var target = source.ToMetric();

            serializer.Serialize(writer, target);
        }
    }
}