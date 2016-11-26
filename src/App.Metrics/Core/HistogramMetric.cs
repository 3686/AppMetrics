﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy


using System;
using App.Metrics.Core.Interfaces;
using App.Metrics.Data;
using App.Metrics.Sampling;
using App.Metrics.Sampling.Interfaces;

namespace App.Metrics.Core
{
    public sealed class HistogramMetric : IHistogramMetric
    {
        private readonly IReservoir _reservoir;
        private bool _disposed = false;
        private UserValueWrapper _last;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HistogramMetric" /> class.
        /// </summary>
        /// <param name="samplingType">Type of the reservoir sampling to use.</param>
        /// <param name="sampleSize">The number of samples to keep in the sampling reservoir</param>
        /// <param name="alpha">
        ///     The alpha value, e.g 0.015 will heavily biases the reservoir to the past 5 mins of measurements. The higher the
        ///     value the more biased the reservoir will be towards newer values.
        /// </param>
        public HistogramMetric(SamplingType samplingType, int sampleSize, double alpha)
            : this(SamplingTypeToReservoir(samplingType, sampleSize, alpha))
        {
        }

        public HistogramMetric(IReservoir reservoir)
        {
            _reservoir = reservoir;
        }

        ~HistogramMetric()
        {
            Dispose(false);
        }

        public HistogramValue Value => GetValue();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                }
            }

            _disposed = true;
        }

        /// <inheritdoc />
        public HistogramValue GetValue(bool resetMetric = false)
        {
            var value = new HistogramValue(_last.Value, _last.UserValue, _reservoir.GetSnapshot(resetMetric));
            if (resetMetric)
            {
                _last = UserValueWrapper.Empty;
            }
            return value;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _last = UserValueWrapper.Empty;
            _reservoir.Reset();
        }

        /// <inheritdoc />
        public void Update(long value, string userValue = null)
        {
            _last = new UserValueWrapper(value, userValue);
            _reservoir.Update(value, userValue);
        }

        private static IReservoir SamplingTypeToReservoir(SamplingType samplingType, int sampleSize, double alpha)
        {
            while (true)
            {
                switch (samplingType)
                {
                    case SamplingType.HighDynamicRange:
                        return new HdrHistogramReservoir();
                    case SamplingType.ExponentiallyDecaying:
                        return new ExponentiallyDecayingReservoir(sampleSize, alpha);
                    case SamplingType.LongTerm:
                        return new UniformReservoir(sampleSize);
                    case SamplingType.SlidingWindow:
                        return new SlidingWindowReservoir(sampleSize);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(samplingType), samplingType, "Sampling type not implemented " + samplingType);
                }
            }
        }
    }
}