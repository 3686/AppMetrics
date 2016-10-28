﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


// Originally Written by Iulian Margarintescu https://github.com/etishor/Metrics.NET
// Ported/Refactored to .NET Standard Library by Allan Hardy


using System;
using System.Collections.Generic;

namespace App.Metrics.MetricData
{
    public struct CounterValue
    {
        public static readonly IComparer<SetItem> SetItemComparer = Comparer<SetItem>.Create((x, y) =>
        {
            var percent = Comparer<double>.Default.Compare(x.Percent, y.Percent);
            return percent == 0 ? Comparer<string>.Default.Compare(x.Item, y.Item) : percent;
        });

        /// <summary>
        ///     Total count of the counter instance.
        /// </summary>
        public readonly long Count;

        /// <summary>
        ///     Separate counters for each registered set item.
        /// </summary>
        public readonly SetItem[] Items;

        private static readonly SetItem[] NoItems = new SetItem[0];

        public CounterValue(long count, SetItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Count = count;
            Items = items;
        }

        internal CounterValue(long count) : this(count, NoItems)
        {
        }


        public struct SetItem
        {
            /// <summary>
            ///     Specific count for this item.
            /// </summary>
            public readonly long Count;

            /// <summary>
            ///     Registered item name.
            /// </summary>
            public readonly string Item;

            /// <summary>
            ///     Percent of this item from the total count.
            /// </summary>
            public readonly double Percent;

            public SetItem(string item, long count, double percent)
            {
                Item = item;
                Count = count;
                Percent = percent;
            }
        }
    }

    /// <summary>
    ///     Combines the value for a counter with the defined unit for the value.
    /// </summary>
    public sealed class CounterValueSource : MetricValueSource<CounterValue>
    {
        public CounterValueSource(string name, IMetricValueProvider<CounterValue> value, Unit unit, MetricTags tags)
            : base(name, value, unit, tags)
        {
        }
    }
}