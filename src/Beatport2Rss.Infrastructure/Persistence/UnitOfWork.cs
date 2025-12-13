using Beatport2Rss.Contracts.Persistence;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class UnitOfWork(Beatport2RssDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}