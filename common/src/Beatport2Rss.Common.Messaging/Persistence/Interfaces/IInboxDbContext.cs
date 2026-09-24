using Beatport2Rss.Common.Messaging.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Interfaces;

public interface IInboxDbContext
{
    DbSet<InboxMessage> InboxMessages { get; }
}