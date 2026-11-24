using Beatport2Rss.Api.Application.Interfaces.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence;

internal sealed class TransactionFactory(ApiDbContext dbContext) : ITransactionFactory
{
    public async Task<IAsyncDisposable> BeginAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Database.BeginTransactionAsync(cancellationToken);
}
