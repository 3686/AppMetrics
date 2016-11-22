﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Text;
using App.Metrics.Core;
using App.Metrics.Data;

namespace App.Metrics.Formatting.Humanize
{
    public static class HumanizingMetricsStringBuilderExtensions
    {
        public static void WriteEndMetricType(this StringBuilder buffer, Type metricType)
        {
            buffer.AppendFormat(metricType.HumanzeEndMetricType());
        }

        public static void WriteEnvironmentInfo(this StringBuilder buffer, EnvironmentInfo environmentInfo)
        {
            buffer.AppendLine(environmentInfo.Hummanize());
        }

        public static void WriteFailedHealthChecksHeader(this StringBuilder buffer)
        {
            buffer.AppendLine("\tFAILED CHECKS");
        }

        public static void WriteHealthCheckResult(this StringBuilder buffer, HealthCheck.Result healthCheckResult)
        {
            buffer.AppendLine(Environment.NewLine + healthCheckResult.Hummanize());
        }

        public static void WriteHealthStatus(this StringBuilder buffer, string status)
        {
            buffer.AppendLine(Environment.NewLine + "\tHealth Status = {status}" + Environment.NewLine);
        }

        public static void WriteMetricEndReport(this StringBuilder buffer, string reportName, string timeStamp)
        {
            buffer.AppendFormat(Environment.NewLine + "-- End {0} Report: {1} --" + Environment.NewLine,
                reportName, timeStamp);
        }

        public static void WriteMetricName<T>(this StringBuilder buffer, string context, MetricValueSource<T> valueSource)
        {
            buffer.AppendLine(valueSource.HumanzizeName(context));
        }

        public static void WriteMetricStartReport(this StringBuilder buffer, string reportName, string timeStamp)
        {
            buffer.AppendFormat(Environment.NewLine + "-- Start {0} Report: {1} --" + Environment.NewLine,
                reportName, timeStamp);
        }

        public static void WriteMetricValue<T>(this StringBuilder buffer, MetricValueSource<T> valueSource)
        {
            buffer.AppendLine(valueSource.Hummanize());
        }

        public static void WritePassedHealthChecksHeader(this StringBuilder buffer)
        {
            buffer.AppendLine("\tPASSED CHECKS");
        }

        public static void WriteDegradedHealthChecksHeader(this StringBuilder buffer)
        {
            buffer.AppendLine("\tDEGRADED CHECKS");
        }

        public static void WriteStartMetricType(this StringBuilder buffer, Type metricType)
        {
            buffer.AppendFormat(metricType.HumanzeStartMetricType());
        }
    }
}