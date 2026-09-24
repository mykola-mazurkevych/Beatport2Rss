using Beatport2Rss.Common.Messaging.Persistence.Entities;

namespace Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

internal interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxMessage>> GetNotPublishedAsync(int batchSize, CancellationToken cancellationToken = default);
}