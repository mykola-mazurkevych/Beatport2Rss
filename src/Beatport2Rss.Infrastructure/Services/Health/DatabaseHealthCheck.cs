using Beatport2Rss.Infrastructure.Persistence;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beatport2Rss.Infrastructure.Services.Health;

internal sealed class DatabaseHealthCheck(Beatport2RssDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded();
    }
}