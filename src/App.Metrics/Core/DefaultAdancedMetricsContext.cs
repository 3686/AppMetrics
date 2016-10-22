using System;
using App.Metrics.DataProviders;
using App.Metrics.Internal;
using App.Metrics.MetricData;
using App.Metrics.Registries;
using App.Metrics.Sampling;
using App.Metrics.Utils;

namespace App.Metrics.Core
{
    internal sealed class DefaultAdancedMetricsContext : IAdvancedMetricsContext
    {
        private readonly IMetricsBuilder _builder;
        private bool _isDisabled = false;
        private IMetricsRegistry _registry;

        public DefaultAdancedMetricsContext(IMetricsContext context,
            IClock clock,
            IMetricsRegistry registry,
            IMetricsBuilder builder,
            IHealthCheckManager healthCheckManager,
            IMetricsDataManager metricsDataManager)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _registry = registry;
            _builder = builder;
            HealthCheckManager = healthCheckManager;
            MetricsDataManager = metricsDataManager;
            Clock = clock;
        }


        public event EventHandler ContextDisabled;

        public event EventHandler ContextShuttingDown;

        public IClock Clock { get; }

        public IHealthCheckManager HealthCheckManager { get; }

        public IMetricsDataManager MetricsDataManager { get; }

        public void CompletelyDisableMetrics()
        {
            if (_isDisabled)
            {
                return;
            }

            _isDisabled = true;

            var oldRegistry = _registry;
            _registry = new NullMetricsRegistry();
            oldRegistry.Clear();
            //TODO: AH - does the registry need to be disposable
            using (oldRegistry as IDisposable)
            {
            }

            ContextShuttingDown?.Invoke(this, EventArgs.Empty);
            ContextDisabled?.Invoke(this, EventArgs.Empty);
        }

        public ICounter Counter<T>(CounterOptions options, Func<T> builder) where T : ICounterImplementation
        {
            return _registry.Counter(options, builder);
        }

        public ICounter Counter(CounterOptions options)
        {
            return Counter(options, () => _builder.BuildCounter(options.Name, options.MeasurementUnit));
        }

        public void Gauge(GaugeOptions options, Func<double> valueProvider)
        {
            Gauge(options, () => _builder.BuildGauge(options.Name, options.MeasurementUnit, valueProvider));
        }

        public void Gauge(GaugeOptions options, Func<IMetricValueProvider<double>> valueProvider)
        {
            _registry.Gauge(options, valueProvider);
        }

        public IHistogram Histogram(HistogramOptions options)
        {
            return Histogram(options, () => _builder.BuildHistogram(options.Name, options.MeasurementUnit, options.SamplingType));
        }

        public IHistogram Histogram<T>(HistogramOptions options, Func<T> builder) where T : IHistogramImplementation
        {
            //NOTE: Options Resevoir will be ignored the builder should specify
            //TODO: AH - ^ bit confusing
            return _registry.Histogram(options, builder);
        }

        public IHistogram Histogram(HistogramOptions options, Func<IReservoir> builder)
        {
            //NOTE: Options Resevoir will be ignored since we're defining it with the builder
            //TODO: AH - ^ bit confusing
            return Histogram(options, () => _builder.BuildHistogram(options.Name, options.MeasurementUnit, builder()));
        }

        public IMeter Meter(MeterOptions options)
        {
            return Meter(options, () => _builder.BuildMeter(options.Name, options.MeasurementUnit, options.RateUnit));
        }

        public IMeter Meter<T>(MeterOptions options, Func<T> builder) where T : IMeterImplementation
        {
            return _registry.Meter(options, builder);
        }

        public void ResetMetricsValues()
        {
            _registry.Clear();
        }

        public void ShutdownGroup(string groupName)
        {
           _registry.RemoveGroup(groupName);
        }

        public ITimer Timer(TimerOptions options)
        {
            return _registry.Timer(options,
                () => _builder.BuildTimer(options.Name, options.MeasurementUnit, options.RateUnit, options.DurationUnit, options.SamplingType));
        }

        public ITimer Timer<T>(TimerOptions options, Func<T> builder) where T : ITimerImplementation
        {
            return _registry.Timer(options, builder);
        }

        public ITimer Timer(TimerOptions options, Func<IHistogramImplementation> builder)
        {
            return Timer(options, () => _builder.BuildTimer(options.Name, options.MeasurementUnit, options.RateUnit, options.DurationUnit, builder()));
        }

        public ITimer Timer(TimerOptions options, Func<IReservoir> builder)
        {
            return Timer(options, () => _builder.BuildTimer(options.Name, options.MeasurementUnit, options.RateUnit, options.DurationUnit, builder()));
        }
    }
}