using Beatport2Rss.Common.Messaging.Persistence.Entities;

namespace Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

internal interface IInboxMessageRepository
{
    Task<InboxMessage> AddAsync(InboxMessage inboxMessage, CancellationToken cancellationToken = default);
}