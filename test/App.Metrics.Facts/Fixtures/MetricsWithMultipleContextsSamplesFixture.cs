using App.Metrics.Core;
using App.Metrics.Core.Options;

namespace App.Metrics.Facts.Fixtures
{
    public class MetricsWithMultipleContextsSamplesFixture : MetricsFixture
    {
        public MetricsWithMultipleContextsSamplesFixture()
        {
            RecordSomeMetrics();
        }

        private void RecordSomeMetrics()
        {
            var counterOptions = new CounterOptions
            {
                Name = "test_counter",
                MeasurementUnit = Unit.Requests,
                Tags = new MetricTags().With("tag1", "value"),
                Context = "test_context1"
            };

            var meterOptions = new MeterOptions
            {
                Name = "test_meter",
                MeasurementUnit = Unit.None,
                Tags = new MetricTags().With("tag2", "value")
            };

            var timerOptions = new TimerOptions
            {
                Name = "test_timer",
                MeasurementUnit = Unit.Requests,
                Context = "test_context2"
            };

            var histogramOptions = new HistogramOptions
            {
                Name = "test_histogram",
                MeasurementUnit = Unit.Requests
            };

            var gaugeOptions = new GaugeOptions
            {
                Name = "test_gauge"
            };

            Metrics.Counter.Increment(counterOptions);
            Metrics.Meter.Mark(meterOptions);
            Metrics.Timer.Time(timerOptions, () => Metrics.Advanced.Clock.Advance(TimeUnit.Milliseconds, 10));
            Metrics.Histogram.Update(histogramOptions, 5);
            Metrics.Gauge.SetValue(gaugeOptions, () => 8);
        }
    }
}