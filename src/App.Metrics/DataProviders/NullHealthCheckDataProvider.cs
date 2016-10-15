using System.Threading.Tasks;

namespace App.Metrics.DataProviders
{
    public sealed class NullHealthCheckDataProvider : IHealthCheckDataProvider
    {
        public Task<HealthStatus> GetStatusAsync()
        {
            return Task.FromResult(new HealthStatus());
        }
    }
}