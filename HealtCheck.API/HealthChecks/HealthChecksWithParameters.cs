using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.API.HealthChecks
{
    public class HealthChecksWithParameters : IHealthCheck
    {
        private string value1;
        private int value2;

        public HealthChecksWithParameters(string value1, int value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("sıkıntı yok"));
        }
    }
}