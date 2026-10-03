using System.Linq.Expressions;

using Beatport2Rss.Common.Messaging.Persistence.Entities;

namespace Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

internal interface IOutboxMessageRepository
{
    Task<IEnumerable<OutboxMessage>> GetNotPublishedAsync(int batchSize, CancellationToken cancellationToken = default);

    Task<OutboxMessage> AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
    Task DeleteAsync(Expression<Func<OutboxMessage, bool>> predicate, CancellationToken cancellationToken = default);
}