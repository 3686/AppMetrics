// Written by Gil Tene of Azul Systems, and released to the public domain,
// as explained at http://creativecommons.org/publicdomain/zero/1.0/
// 
// Ported to .NET by Iulian Margarintescu under the same license and terms as the java version
// Java Version repo: https://github.com/HdrHistogram/HdrHistogram
// Latest ported version is available in the Java submodule in the root of the repo

// Ported to.NET Standard Library by Allan Hardy

using System;
using App.Metrics.App_Packages.Concurrency;

namespace App.Metrics.App_Packages.HdrHistogram
{
    /// <summary>
    ///     This non-public AbstractHistogramBase super-class separation is meant to bunch "cold" fields
    ///     separately from "hot" fields, in an attempt to force the JVM to place the (hot) fields
    ///     commonly used in the value recording code paths close together.
    ///     Subclass boundaries tend to be strongly control memory layout decisions in most practical
    ///     JVM implementations, making this an effective method for control filed grouping layout.
    /// </summary>
    internal abstract class AbstractHistogramBase
    {
        internal int countsArrayLength;

        // "Cold" accessed fields. Not used in the recording code path:
        internal protected readonly long Identity;
        internal protected readonly int NumberOfSignificantValueDigits;


        internal protected int bucketCount;
        internal protected long endTimeStampMsec = 0;
        internal protected long HighestTrackableValue;

        internal protected double integerToDoubleValueConversionRatio = 1.0;


        internal protected long startTimeStampMsec = long.MaxValue;
        internal protected int subBucketCount;

        protected readonly bool AutoResize;

        protected readonly long LowestDiscernibleValue;

        protected readonly RecordedValuesIterator recordedValuesIterator;
        protected readonly int WordSizeInBytes;
        private static AtomicLong _constructionIdentityCount = new AtomicLong(0);

        protected AbstractHistogramBase(long lowestDiscernibleValue, int numberOfSignificantValueDigits, int wordSizeInBytes, bool autoResize)
        {
            // Verify argument validity
            if (lowestDiscernibleValue < 1)
            {
                throw new ArgumentException("lowestDiscernibleValue must be >= 1");
            }

            if ((numberOfSignificantValueDigits < 0) || (numberOfSignificantValueDigits > 5))
            {
                throw new ArgumentException("numberOfSignificantValueDigits must be between 0 and 5");
            }

            LowestDiscernibleValue = lowestDiscernibleValue;
            Identity = _constructionIdentityCount.GetAndIncrement();
            NumberOfSignificantValueDigits = numberOfSignificantValueDigits;
            WordSizeInBytes = wordSizeInBytes;
            AutoResize = autoResize;

            recordedValuesIterator = new RecordedValuesIterator(this as AbstractHistogram);
        }
    }
}