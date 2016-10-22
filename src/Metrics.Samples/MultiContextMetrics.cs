﻿using App.Metrics;

namespace Metrics.Samples
{
    public class MultiContextMetrics
    {
        private readonly ICounter _firstCounter;
        private readonly ICounter _secondCounter;
        private readonly IMeter _secondMeter;

        public MultiContextMetrics(IMetricsContext metricsContext)
        {
            _firstCounter = metricsContext.Advanced.Counter(SampleMetricsRegistry.Groups.FirstGroup.Counters.Counter);
            _secondCounter = metricsContext.Advanced.Counter(SampleMetricsRegistry.Groups.SecondGroup.Counters.Counter);
            _secondMeter = metricsContext.Advanced.Meter(SampleMetricsRegistry.Groups.SecondGroup.Meters.Requests);
        }

        public void Run()
        {
            _firstCounter.Increment();
            _secondCounter.Increment();
            _secondMeter.Mark();
        }
    }

    public class MultiContextInstanceMetrics
    {
        private readonly ICounter _instanceCounter;
        private readonly ITimer _instanceTimer;
        private static IMetricsContext _metricsContext;

        public MultiContextInstanceMetrics(string instanceName, IMetricsContext metricsContext)
        {
            _metricsContext = metricsContext;

            //TODO: AH - no longer valid?
            //var context = _metricsContext.Advanced.Group(instanceName);

            _instanceCounter = _metricsContext.Advanced.Counter(SampleMetricsRegistry.Counters.SampleCounter);
            _instanceTimer = _metricsContext.Advanced.Timer(SampleMetricsRegistry.Timers.SampleTimer);
        }

        public void Run()
        {
            using (var context = this._instanceTimer.NewContext())
            {
                _instanceCounter.Increment();
            }
        }

        public void RunSample()
        {
            for (var i = 0; i < 5; i++)
            {
                new MultiContextInstanceMetrics("Sample Instance " + i.ToString(), _metricsContext).Run();
            }
        }
    }
}