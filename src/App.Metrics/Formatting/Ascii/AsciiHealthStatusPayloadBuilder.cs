﻿// <copyright file="AsciiHealthStatusPayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.IO;
using App.Metrics.Health;
using App.Metrics.Reporting.Abstractions;

namespace App.Metrics.Formatting.Ascii
{
    // TODO: Remove in 2.0.0 - Add to separate formatting package
    public class AsciiHealthStatusPayloadBuilder : IHealthStatusPayloadBuilder<AsciiHealthStatusPayload>
    {
        private AsciiHealthStatusPayload _payload;

        public AsciiHealthStatusPayloadBuilder() { _payload = new AsciiHealthStatusPayload(); }

        public void Clear() { _payload = null; }

        /// <inheritdoc />
        public void Init()
        {
            _payload = new AsciiHealthStatusPayload();
        }

        /// <inheritdoc />
        public void Pack(string name, string message, HealthCheckStatus status)
        {
            _payload.Add(new AsciiHealthCheckResult(name, message, status));
        }

        /// <inheritdoc />
        public AsciiHealthStatusPayload Payload()
        {
            return _payload;
        }

        /// <inheritdoc />
        public string PayloadFormatted()
        {
            var result = new StringWriter();
            _payload.Format(result);
            return result.ToString();
        }
    }
}