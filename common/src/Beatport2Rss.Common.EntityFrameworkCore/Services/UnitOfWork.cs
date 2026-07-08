using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.EntityFrameworkCore.Services;

internal sealed class UnitOfWork(DbContext dbContext) :
    IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}