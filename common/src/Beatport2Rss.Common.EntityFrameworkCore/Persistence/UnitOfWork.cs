using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence;

internal sealed class UnitOfWork(DbContext dbContext) :
    IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}