using Beatport2Rss.Api.Application.Interfaces.Persistence;

namespace Beatport2Rss.Api.Infrastructure.Persistence;

internal sealed class UnitOfWork(Beatport2RssDbContext dbContext) :
    IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}