using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Subscription, SubscriptionId>(dbContext),
    ISubscriptionCommandRepository;