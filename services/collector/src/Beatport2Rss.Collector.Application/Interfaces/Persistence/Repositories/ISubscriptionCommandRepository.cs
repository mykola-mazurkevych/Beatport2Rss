using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository
{
    Task<bool> ExistsAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SubscriptionType type, BeatportId beatportId, CancellationToken cancellationToken = default);

    Task<Subscription?> FindAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
    Task<bool> HasFeedsAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SubscriptionId>> FindAllToRefreshAsync(CancellationToken cancellationToken = default);

    Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    void Update(Subscription subscription);
}