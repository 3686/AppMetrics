﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using App.Metrics.App_Packages.Concurrency;
using App.Metrics.MetricData;

namespace App.Metrics.Core
{
    public sealed class CounterMetric : ICounterImplementation
    {
        private readonly StripedLongAdder _counter = new StripedLongAdder();
        private ConcurrentDictionary<string, StripedLongAdder> _setCounters;

        public CounterValue Value
        {
            get
            {
                if (_setCounters == null || _setCounters.Count == 0)
                {
                    return new CounterValue(_counter.GetValue());
                }
                return GetValueWithSetItems();
            }
        }

        public void Decrement()
        {
            _counter.Decrement();
        }

        public void Decrement(long value)
        {
            _counter.Add(-value);
        }

        public void Decrement(string item)
        {
            Decrement();
            SetCounter(item).Decrement();
        }

        public void Decrement(string item, long amount)
        {
            Decrement(amount);
            SetCounter(item).Add(-amount);
        }

        public CounterValue GetValue(bool resetMetric = false)
        {
            var value = Value;
            if (resetMetric)
            {
                Reset();
            }
            return value;
        }

        public void Increment()
        {
            _counter.Increment();
        }

        public void Increment(long value)
        {
            _counter.Add(value);
        }

        public void Increment(string item)
        {
            Increment();
            SetCounter(item).Increment();
        }

        public void Increment(string item, long amount)
        {
            Increment(amount);
            SetCounter(item).Add(amount);
        }

        public void Reset()
        {
            _counter.Reset();
            if (_setCounters != null)
            {
                foreach (var item in _setCounters)
                {
                    item.Value.Reset();
                }
            }
        }

        private CounterValue GetValueWithSetItems()
        {
            Debug.Assert(_setCounters != null);
            var total = _counter.GetValue();

            var items = new CounterValue.SetItem[_setCounters.Count];
            var index = 0;
            foreach (var item in _setCounters)
            {
                var itemValue = item.Value.GetValue();

                var percent = total > 0 ? itemValue / (double)total * 100 : 0.0;
                var setCounter = new CounterValue.SetItem(item.Key, itemValue, percent);
                items[index++] = setCounter;
                if (index == items.Length)
                {
                    break;
                }
            }

            Array.Sort(items, CounterValue.SetItemComparer);

            return new CounterValue(total, items);
        }

        private StripedLongAdder SetCounter(string item)
        {
            if (_setCounters == null)
            {
                Interlocked.CompareExchange(ref _setCounters, new ConcurrentDictionary<string, StripedLongAdder>(), null);
            }
            Debug.Assert(_setCounters != null);
            return _setCounters.GetOrAdd(item, v => new StripedLongAdder());
        }
    }
}