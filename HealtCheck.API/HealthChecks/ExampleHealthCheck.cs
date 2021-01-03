using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.API.HealthChecks
{
    public class ExampleHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckStatus = true;

            if (healthCheckStatus)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Sıkıntı yok"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("bir problem var"));
            }
        }
    }
}