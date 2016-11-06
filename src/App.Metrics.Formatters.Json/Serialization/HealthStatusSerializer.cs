using App.Metrics.Utils;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Json
{
    public class HealthStatusSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public HealthStatusSerializer(IClock clock)
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new MetricContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            _settings.Converters.Add(new HealthStatusConverter(clock));
        }

        public virtual T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public virtual string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }
    }
}