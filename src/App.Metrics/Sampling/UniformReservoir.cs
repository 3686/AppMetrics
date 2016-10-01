﻿// Written by Iulian Margarintescu
// 
// Ported to .NET Standard Library by Allan Hardy
// Original repo: https://github.com/etishor/Metrics.NET

using System;
using System.Linq;
using App.Metrics.App_Packages.Concurrency;

namespace App.Metrics.Sampling
{
    public sealed class UniformReservoir : Reservoir
    {
        private const int DefaultSize = 1028;

        private readonly UserValueWrapper[] _values;

        private AtomicLong _count = new AtomicLong();

        public UniformReservoir()
            : this(DefaultSize)
        {
        }

        public UniformReservoir(int size)
        {
            _values = new UserValueWrapper[size];
        }

        public int Size => Math.Min((int)_count.GetValue(), _values.Length);

        public Snapshot GetSnapshot(bool resetReservoir = false)
        {
            var size = Size;
            if (size == 0)
            {
                return new UniformSnapshot(0, Enumerable.Empty<long>());
            }

            var snapshotValues = new UserValueWrapper[size];
            Array.Copy(_values, snapshotValues, size);

            if (resetReservoir)
            {
                _count.SetValue(0L);
            }

            Array.Sort(snapshotValues, UserValueWrapper.Comparer);
            var minValue = snapshotValues[0].UserValue;
            var maxValue = snapshotValues[size - 1].UserValue;
            return new UniformSnapshot(_count.GetValue(), snapshotValues.Select(v => v.Value), valuesAreSorted: true, minUserValue: minValue,
                maxUserValue: maxValue);
        }

        public void Reset()
        {
            _count.SetValue(0L);
        }

        public void Update(long value, string userValue = null)
        {
            var c = _count.Increment();
            if (c <= _values.Length)
            {
                _values[(int)c - 1] = new UserValueWrapper(value, userValue);
            }
            else
            {
                var r = ThreadLocalRandom.NextLong(c);
                if (r < _values.Length)
                {
                    _values[(int)r] = new UserValueWrapper(value, userValue);
                }
            }
        }
    }
}