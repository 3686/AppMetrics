using App.Metrics.MetricData;

namespace App.Metrics.Core
{
    public interface ITimerImplementation : ITimer, IMetricValueProvider<TimerValue>
    {
    }
}