using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Infrastructure.Persistence;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class DatabaseHealthService(Beatport2RssDbContext dbContext) : IDatabaseHealthService
{
    public Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default) =>
        dbContext.Database.CanConnectAsync(cancellationToken);
}