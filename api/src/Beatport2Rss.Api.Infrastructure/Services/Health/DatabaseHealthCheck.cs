using Beatport2Rss.Api.Infrastructure.Persistence;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beatport2Rss.Api.Infrastructure.Services.Health;

internal sealed class DatabaseHealthCheck(ApiDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded();
    }
}