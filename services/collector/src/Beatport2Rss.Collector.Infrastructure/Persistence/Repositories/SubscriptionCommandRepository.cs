using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(
    DbSet<Subscription> subscriptions,
    IClock clock) :
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

    public async Task<IReadOnlyCollection<SubscriptionId>> FindAllToRefreshAsync(CancellationToken cancellationToken = default) =>
        await _subscriptions
            .Where(subscription => subscription.SubscribersCount > 0)
            .Where(subscription => subscription.RefreshedAt != clock.UtcToday)
            .OrderBy(subscription => subscription.RefreshedAt ?? DateOnly.MinValue)
            .Select(subscription => subscription.Id)
            .ToArrayAsync(cancellationToken);
}