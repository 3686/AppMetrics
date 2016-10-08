﻿using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Core;

namespace Metrics.Samples
{
    public interface IDatabase
    {
        void Ping();
    }

    public class Database : IDatabase
    {
        public void Ping()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DatabaseHealthCheck : HealthCheck
    {
        private readonly IDatabase _database;

        public DatabaseHealthCheck(IDatabase database)
            : base("DatabaseCheck")
        {
            _database = database;
        }

        protected override Task<HealthCheckResult> CheckAsync()
        {
            // exceptions will be caught and 
            // the result will be unhealthy
            _database.Ping();

            //TODO: AH - Add healthly and unhealthy to task cache
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}