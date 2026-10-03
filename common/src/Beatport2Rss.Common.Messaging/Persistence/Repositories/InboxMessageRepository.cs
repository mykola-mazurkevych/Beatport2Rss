using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;
using Beatport2Rss.Common.Messaging.Persistence.Entities;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Repositories;

internal sealed class InboxMessageRepository(DbSet<InboxMessage> inboxMessages) :
    CommandRepository<InboxMessage>(inboxMessages),
    IInboxMessageRepository;