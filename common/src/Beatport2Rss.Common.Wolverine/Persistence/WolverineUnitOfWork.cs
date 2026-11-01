using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

using Wolverine.EntityFrameworkCore;

namespace Beatport2Rss.Common.Wolverine.Persistence;

internal sealed class WolverineUnitOfWork<TDbContext>(IDbContextOutbox<TDbContext> outbox) :
    IUnitOfWork
    where TDbContext : DbContext
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
}