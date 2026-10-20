using Beatport2Rss.Collector.Domain.Subscriptions;

namespace Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository
{
    Task<bool> ExistsAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default);

    Task<Subscription?> FindAsync(
        SubscriptionId subscriptionId,
        CancellationToken cancellationToken = default);
    
    Task<Subscription> AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);

    void Update(Subscription subscription);
}