using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> subscriptions) :
    CommandRepository<Subscription, SubscriptionId>(subscriptions),
    ISubscriptionCommandRepository
{
    private readonly DbSet<Subscription> _subscriptions = subscriptions;

    public Task<bool> ExistsAsync(SubscriptionType type, BeatportId beatportId, CancellationToken cancellationToken = default) =>
        _subscriptions.Local.Any(subscription => subscription.Type == type && subscription.BeatportId == beatportId)
            ? Task.FromResult(true)
            : ExistsAsync(
                subscription =>
                    subscription.Type == type &&
                    subscription.BeatportId == beatportId,
                cancellationToken);

    public Task<Subscription?> FindAsync(SubscriptionType type, BeatportId beatportId, CancellationToken cancellationToken = default) =>
        FindAsync(
            subscription =>
                subscription.Type == type &&
                subscription.BeatportId == beatportId,
            cancellationToken);
}