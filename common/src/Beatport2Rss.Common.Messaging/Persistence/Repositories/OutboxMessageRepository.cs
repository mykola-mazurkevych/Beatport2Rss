using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Repositories;

internal sealed class OutboxMessageRepository(IOutboxDbContext dbContext) :
    IOutboxMessageRepository
{
    public Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default) =>
        dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken).AsTask();

    public async Task<IEnumerable<OutboxMessage>> GetNotPublishedAsync(int batchSize, CancellationToken cancellationToken = default) =>
        (await dbContext.OutboxMessages
            .Where(message => !message.IsPublished)
            .OrderBy(message => message.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken))
        .AsEnumerable();
}