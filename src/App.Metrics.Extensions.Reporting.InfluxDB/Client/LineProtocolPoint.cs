﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Metrics.Extensions.Reporting.InfluxDB.Client
{
    public class LineProtocolPoint
    {
        public LineProtocolPoint(
            string measurement,
            IReadOnlyDictionary<string, object> fields,
            IReadOnlyDictionary<string, string> tags = null,
            DateTime? utcTimestamp = null)
        {
            if (string.IsNullOrEmpty(measurement)) throw new ArgumentException("A measurement name must be specified");
            if (fields == null || fields.Count == 0) throw new ArgumentException("At least one field must be specified");

            if (fields.Any(f => string.IsNullOrEmpty(f.Key)))
            {
                throw new ArgumentException("Fields must have non-empty names");
            }

            if (tags != null)
            {
                if (tags.Any(t => string.IsNullOrEmpty(t.Key)))
                {
                    throw new ArgumentException("Tags must have non-empty names");
                }
            }


            if (utcTimestamp != null && utcTimestamp.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Timestamps must be specified as UTC");
            }

            Measurement = measurement;
            Fields = fields;
            Tags = tags;
            UtcTimestamp = utcTimestamp;
        }

        public IReadOnlyDictionary<string, object> Fields { get; }

        public string Measurement { get; }

        public IReadOnlyDictionary<string, string> Tags { get; }

        public DateTime? UtcTimestamp { get; }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));

            textWriter.Write(LineProtocolSyntax.EscapeName(Measurement));

            if (Tags != null)
            {
                foreach (var t in Tags.OrderBy(t => t.Key))
                {
                    if (string.IsNullOrEmpty(t.Value))
                    {
                        continue;
                    }

                    textWriter.Write(',');
                    textWriter.Write(LineProtocolSyntax.EscapeName(t.Key));
                    textWriter.Write('=');
                    textWriter.Write(LineProtocolSyntax.EscapeName(t.Value));
                }
            }

            var fieldDelim = ' ';
            foreach (var f in Fields)
            {
                textWriter.Write(fieldDelim);
                fieldDelim = ',';
                textWriter.Write(LineProtocolSyntax.EscapeName(f.Key));
                textWriter.Write('=');
                textWriter.Write(LineProtocolSyntax.FormatValue(f.Value));
            }

            if (UtcTimestamp == null)
            {
                return;
            }

            textWriter.Write(' ');
            textWriter.Write(LineProtocolSyntax.FormatTimestamp(UtcTimestamp.Value));
        }
    }
}