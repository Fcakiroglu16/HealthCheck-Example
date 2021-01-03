using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.API.HealthChecks
{
    public class ConsolePublisher : IHealthCheckPublisher
    {
        private readonly ILogger _logger;

        public ConsolePublisher(ILogger<ConsolePublisher> logger)
        {
            _logger = logger;
        }

        // The following example is for demonstration purposes only. Health Checks
        // Middleware already logs health checks results. A real-world readiness
        // check in a production app might perform a set of more expensive or
        // time-consuming checks to determine if other resources are responding
        // properly.
        public Task PublishAsync(HealthReport report,
            CancellationToken cancellationToken)
        {
            if (report.Status == HealthStatus.Healthy)
            {
                _logger.LogInformation("{Timestamp} Readiness Probe Status: {Result}",
                    DateTime.UtcNow, report.Status);
            }
            else
            {
                _logger.LogError("{Timestamp} Readiness Probe Status: {Result}",
                    DateTime.UtcNow, report.Status);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }
    }
}