using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> subscriptions) :
    CommandRepository<Subscription, SubscriptionId>(subscriptions),
    ISubscriptionCommandRepository
{
    public Task<bool> ExistsAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default) =>
        ExistsAsync(subscription => subscription.Id == subscriptionId, cancellationToken);
}