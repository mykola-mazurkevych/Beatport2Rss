using System.Linq.Expressions;

using Beatport2Rss.Common.Messaging.Persistence.Entities;

namespace Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

internal interface IInboxMessageRepository
{
    Task<InboxMessage> AddAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default);
    Task DeleteAsync(Expression<Func<InboxMessage, bool>> predicate, CancellationToken cancellationToken = default);
}