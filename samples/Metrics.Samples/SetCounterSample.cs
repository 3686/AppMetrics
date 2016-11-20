﻿using System;
using App.Metrics;

namespace Metrics.Samples
{
    public class SetCounterSample
    {
        private readonly ICounter _commandCounter;
        private static IMetrics _metrics;

        public SetCounterSample(IMetrics metrics)
        {
            _metrics = metrics;

            _commandCounter = _metrics.Advanced.Counter(SampleMetricsRegistry.Counters.CommandCounter);
        }

        public void Process(Command command)
        {
            
            _commandCounter.Increment(command.GetType().Name);
        }

        public void RunSomeRequests()
        {
            for (var i = 0; i < 30; i++)
            {
                var commandIndex = new Random().Next() % 5;
                if (commandIndex == 0) Process(new SendEmail());
                if (commandIndex == 1) Process(new ShipProduct());
                if (commandIndex == 2) Process(new BillCustomer());
                if (commandIndex == 3) Process(new MakeInvoice());
                if (commandIndex == 4) Process(new MarkAsPreffered());
            }
        }

        public interface Command
        {
        }

        public class BillCustomer : Command
        {
        }

        public class MakeInvoice : Command
        {
        }

        public class MarkAsPreffered : Command
        {
        }

        public class SendEmail : Command
        {
        }

        public class ShipProduct : Command
        {
        }
    }
}