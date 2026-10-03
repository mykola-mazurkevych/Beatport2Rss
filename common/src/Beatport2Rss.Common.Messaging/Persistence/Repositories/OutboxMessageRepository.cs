using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;
using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Repositories;

internal sealed class OutboxMessageRepository(DbSet<OutboxMessage> outboxMessages) :
    CommandRepository<OutboxMessage>(outboxMessages),
    IOutboxMessageRepository
{
    private readonly DbSet<OutboxMessage> _outboxMessages = outboxMessages;

    public async Task<IEnumerable<OutboxMessage>> GetNotPublishedAsync(int batchSize, CancellationToken cancellationToken = default) =>
        (await _outboxMessages
            .Where(message => !message.PublishedAt.HasValue)
            .OrderBy(message => message.OccurredAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken))
        .AsEnumerable();
}