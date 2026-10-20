using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> dbSet) :
    CommandRepository<Subscription, SubscriptionId>(dbSet),
    ISubscriptionCommandRepository
{
    public Task<bool> ExistsAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default) =>
        base.ExistsAsync(s => s.Id == subscriptionId, cancellationToken);

    public Task<Subscription?> FindAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default) =>
        base.FindAsync(s => s.Id == subscriptionId, cancellationToken);
}