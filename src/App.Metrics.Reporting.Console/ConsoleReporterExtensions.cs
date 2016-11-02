// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace App.Metrics.Reporting
{
    public static class ConsoleReporterExtensions
    {
        public static IReportFactory AddConsole(this IReportFactory factory,
            IConsoleReporterSettings settings)
        {
            factory.AddProvider(new ConsoleReporterProvider(settings));
            return factory;
        }

        public static IReportFactory AddConsole(this IReportFactory factory,
            TimeSpan interval)
        {
            var settings = new ConsoleReporterSettings();
            factory.AddConsole(settings);
            return factory;
        }
    }
}