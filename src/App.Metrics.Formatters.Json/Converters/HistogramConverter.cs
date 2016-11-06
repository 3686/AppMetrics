using System;
using App.Metrics.Json;
using App.Metrics.MetricData;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Json
{
    public class HistogramConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(HistogramValueSource) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var source = serializer.Deserialize<JsonHistogram>(reader);

            return source.FromJson();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var source = (HistogramValueSource)value;

            var target = source.ToJson();

            serializer.Serialize(writer, target);
        }
    }
}