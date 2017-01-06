﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


// Ported to.NET Standard Library by Allan Hardy

using System;

namespace App.Metrics.Sampling.HdrHistogram
{
    // ReSharper disable ArrangeModifiersOrder
    // ReSharper disable ArrangeThisQualifier

    /// <summary>
    ///     Used for iterating through histogram values.
    /// </summary>
    internal abstract class AbstractHistogramIterator : Iterator<HistogramIterationValue>
    {
        internal readonly HistogramIterationValue CurrentIterationValue = new HistogramIterationValue();

        protected long ArrayTotalCount;
        protected long CountAtThisValue;

        protected int CurrentIndex;
        protected long CurrentValueAtIndex;
        protected AbstractHistogram Histogram;

        protected long NextValueAtIndex;
        protected long SavedHistogramTotalRawCount;

        protected long TotalCountToCurrentIndex;

        private bool _freshSubBucket;

        private double _integerToDoubleValueConversionRatio;

        long _prevValueIteratedTo;
        long _totalCountToPrevIndex;
        long _totalValueToCurrentIndex;

        /**
         * Returns true if the iteration has more elements. (In other words, returns true if next would return an
         * element rather than throwing an exception.)
         *
         * @return true if the iterator has more elements.
         */

        public override bool hasNext()
        {
            if (Histogram.getTotalCount() != SavedHistogramTotalRawCount)
            {
                throw new InvalidOperationException("ConcurrentModificationException");
            }
            return (TotalCountToCurrentIndex < ArrayTotalCount);
        }

        /**
         * Returns the next element in the iteration.
         *
         * @return the {@link HistogramIterationValue} associated with the next element in the iteration.
         */

        public override HistogramIterationValue next()
        {
            // Move through the sub buckets and buckets until we hit the next reporting level:
            while (!ExhaustedSubBuckets())
            {
                CountAtThisValue = Histogram.getCountAtIndex(CurrentIndex);
                if (_freshSubBucket)
                {
                    // Don't add unless we've incremented since last bucket...
                    TotalCountToCurrentIndex += CountAtThisValue;
                    _totalValueToCurrentIndex += CountAtThisValue * Histogram.highestEquivalentValue(CurrentValueAtIndex);
                    _freshSubBucket = false;
                }
                if (ReachedIterationLevel())
                {
                    var valueIteratedTo = GetValueIteratedTo();
                    CurrentIterationValue.Set(valueIteratedTo, _prevValueIteratedTo, CountAtThisValue,
                        (TotalCountToCurrentIndex - _totalCountToPrevIndex), TotalCountToCurrentIndex,
                        _totalValueToCurrentIndex, ((100.0 * TotalCountToCurrentIndex) / ArrayTotalCount),
                        GetPercentileIteratedTo(), _integerToDoubleValueConversionRatio);
                    _prevValueIteratedTo = valueIteratedTo;
                    _totalCountToPrevIndex = TotalCountToCurrentIndex;
                    // move the next iteration level forward:
                    IncrementIterationLevel();
                    if (Histogram.getTotalCount() != SavedHistogramTotalRawCount)
                    {
                        throw new InvalidOperationException("ConcurrentModificationException");
                    }
                    return CurrentIterationValue;
                }
                IncrementSubBucket();
            }
            // Should not reach here. But possible for overflowed histograms under certain conditions
            throw new IndexOutOfRangeException();
        }

        protected abstract void IncrementIterationLevel();

        protected abstract bool ReachedIterationLevel();

        protected void ResetIterator(AbstractHistogram histogram)
        {
            this.Histogram = histogram;
            this.SavedHistogramTotalRawCount = histogram.getTotalCount();
            this.ArrayTotalCount = histogram.getTotalCount();
            this._integerToDoubleValueConversionRatio = histogram.integerToDoubleValueConversionRatio;
            this.CurrentIndex = 0;
            this.CurrentValueAtIndex = 0;
            this.NextValueAtIndex = 1 << histogram.unitMagnitude;
            this._prevValueIteratedTo = 0;
            this._totalCountToPrevIndex = 0;
            this.TotalCountToCurrentIndex = 0;
            this._totalValueToCurrentIndex = 0;
            this.CountAtThisValue = 0;
            this._freshSubBucket = true;
            CurrentIterationValue.Reset();
        }

        private bool ExhaustedSubBuckets()
        {
            return (CurrentIndex >= Histogram.countsArrayLength);
        }

        double GetPercentileIteratedTo()
        {
            return (100.0 * this.TotalCountToCurrentIndex) / ArrayTotalCount;
        }

        long GetValueIteratedTo()
        {
            return Histogram.highestEquivalentValue(CurrentValueAtIndex);
        }

        void IncrementSubBucket()
        {
            _freshSubBucket = true;
            // Take on the next index:
            CurrentIndex++;
            CurrentValueAtIndex = Histogram.ValueFromIndex(CurrentIndex);
            // Figure out the value at the next index (used by some iterators):
            NextValueAtIndex = Histogram.ValueFromIndex(CurrentIndex + 1);
        }
    }
}
// ReSharper restore ArrangeModifiersOrder
// ReSharper restore ArrangeThisQualifier