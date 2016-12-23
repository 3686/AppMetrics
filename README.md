# App Metrics

[![Build status](https://ci.appveyor.com/api/projects/status/r4x0et4g6mr5vttf?svg=true)](https://ci.appveyor.com/project/alhardy/appmetrics/branch/master) [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0) [![Coverage Status](https://coveralls.io/repos/github/alhardy/AppMetrics/badge.svg)](https://coveralls.io/github/alhardy/AppMetrics)

|Package|Pre Release|Latest Release|
|------|:--------:|:--------:|
|App.Metrics|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.svg)](https://www.nuget.org/packages/App.Metrics/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.svg)](https://www.nuget.org/packages/App.Metrics/)
|App.Metrics.Extensions.Mvc|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Extensions.Mvc.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Mvc/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Extensions.Mvc.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Mvc/)
|App.Metrics.Extensions.Middleware|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Extensions.Middleware.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Middleware/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Extensions.Middleware.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Middleware/)
|App.Metrics.Formatters.Json|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Formatters.Json.svg)](https://www.nuget.org/packages/App.Metrics.Formatters.Json/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Formatters.Json.svg)](https://www.nuget.org/packages/App.Metrics.Formatters.Json/)
|App.Metrics.Extensions.Reporting.InfluxDB|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Extensions.Reporting.InfluxDB.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.InfluxDB/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Extensions.Reporting.InfluxDB.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.InfluxDB/)
|App.Metrics.Extensions.Reporting.Console|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Extensions.Reporting.Console.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.Console/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Extensions.Reporting.Console.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.Console/)
|App.Metrics.Extensions.Reporting.TextFile|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Extensions.Reporting.TextFile.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.TextFile/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Extensions.Reporting.TextFile.svg)](https://www.nuget.org/packages/App.Metrics.Extensions.Reporting.TextFile/)|

## What is App Metrics?

App Metrics is an open-source and cross-platform .NET library used to record metrics within an application. App Metrics can run on .NET Core or on the full .NET framework. App Metrics abstracts away the underlaying repository of your Metrics for example InfluxDB, Graphite, Elasticsearch etc, by sampling and aggregating in memory and providing extensibility points to flush metrics to a repository at a specified interval.

App Metrics provides various metric types to measure things such as the rate of requests, counting the number of user logins over time, measure the time taken to execute a database query, measure the amount of free memory and so on. Metrics types supporter are Gauges, Counters, Meters, Histograms and Timers.

App Metrics also provides a health checking system allowing you to monitor the health of your application through user defined checks.

- [Getting Started](https://alhardy.github.io/app-metrics-docs/getting-started/intro.html)
- [Api Documentation](https://alhardy.github.io/app-metrics-docs/api/index.html)
- [Samples](https://github.com/alhardy/AppMetrics/tree/master/samples)

## Acknowledgements

**Built using the following open source projects**

* [ASP.NET Core](https://github.com/aspnet)
* [DocFX](https://dotnet.github.io/docfx/)
* [Json.Net](http://www.newtonsoft.com/json)
* [Fluent Assertions](http://www.fluentassertions.com/)
* [XUnit](https://xunit.github.io/)

----------

**Ported from [Metrics.NET](https://github.com/etishor/Metrics.NET)**

App Metrics is a significant redesign and .NET Standard port of the [Metrics.NET](https://github.com/etishor/Metrics.NET) library, which is a port of the Java [Metrics](https://github.com/dropwizard/metrics) library. This library for now includes the original [sampling code](https://github.com/etishor/Metrics.NET/tree/master/Src/Metrics/Sampling) written by Metrics.NET. 

Metrics.NET features that have been removed for now are the following:

1. Visualization, I believe most will be using something like Grafana to visualize their metrics
2. Remote Metrics
3. Performance counters as they are windows specific

App Metrics includes addition features to Metrics.NET, see the [docs](https://alhardy.github.io/app-metrics-docs/getting-started/intro.html) for details.

Why another .NET port? The main reason for porting Metrics.NET was to have it run on .NET Standard. Intially I refactored Metrics.NET stripping out features which required a fairly large refactor such as visualization, however the maintainers did not see .NET Standard or Core a priority at the time. 

This library will always keep the same license as the original [Metrics.NET Library](https://github.com/etishor/Metrics.NET) (as long as its an open source, permisive license). 

The original metrics project is released under these terms

"Metrics.NET is release under Apache 2.0 License 
Copyright (c) 2014 Iulian Margarintescu"

## License

This library is release under Apache 2.0 License ( see LICENSE ) Copyright (c) 2016 Allan Hardy
