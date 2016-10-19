﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy


using System;
using App.Metrics.Utils;

namespace App.Metrics.MetricData
{
    /// <summary>
    ///     Indicates the ability to provide the value for a metric.
    ///     This is the raw value. Consumers should use <see cref="MetricValueSource{T}" />
    /// </summary>
    /// <typeparam name="T">Type of the value returned by the metric</typeparam>
    public interface IMetricValueProvider<T> : IHideObjectMembers
    {
        /// <summary>
        ///     The current value of the metric.
        /// </summary>
        T Value { get; }

        /// <summary>
        ///     Get the current value for the metric, but also reset the metric.
        ///     Useful for pushing data to only one consumer (ex: graphite) where you might want to only capture values just
        ///     between the report interval.
        /// </summary>
        /// <param name="resetMetric">if set to true the metric will be reset.</param>
        /// <returns>The current value for the metric.</returns>
        T GetValue(bool resetMetric = false);
    }

    public sealed class ScaledValueProvider<T> : IMetricValueProvider<T>
    {
        private readonly Func<T, T> scalingFunction;

        public ScaledValueProvider(IMetricValueProvider<T> valueProvider, Func<T, T> transformation)
        {
            ValueProvider = valueProvider;
            scalingFunction = transformation;
        }

        public T Value => scalingFunction(ValueProvider.Value);

        public IMetricValueProvider<T> ValueProvider { get; }

        public T GetValue(bool resetMetric = false)
        {
            return scalingFunction(ValueProvider.GetValue(resetMetric));
        }
    }

    /// <summary>
    ///     Provides the value of a metric and information about units.
    ///     This is the class that metric consumers should use.
    /// </summary>
    /// <typeparam name="T">Type of the metric value</typeparam>
    public abstract class MetricValueSource<T> : IHideObjectMembers
    {
        protected MetricValueSource(string name, IMetricValueProvider<T> valueProvider, Unit unit, MetricTags tags)
        {
            Name = name;
            Unit = unit;
            ValueProvider = valueProvider;
            Tags = tags.Tags;
        }

        /// <summary>
        ///     Name of the metric.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Tags associated with the metric.
        /// </summary>
        public string[] Tags { get; private set; }

        /// <summary>
        ///     Unit representing what the metric is measuring.
        /// </summary>
        public Unit Unit { get; private set; }

        /// <summary>
        ///     The current value of the metric.
        /// </summary>
        public T Value => ValueProvider.Value;

        /// <summary>
        ///     Instance capable of returning the current value for the metric.
        /// </summary>
        public IMetricValueProvider<T> ValueProvider { get; }
    }
}