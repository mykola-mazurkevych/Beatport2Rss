using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository
{
    Task<bool> ExistsAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);

    Task<Subscription?> FindAsync(SubscriptionId subscriptionId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Subscription>> FindAllAsync(IEnumerable<BeatportId> beatportIds, CancellationToken cancellationToken = default);

    Task<Subscription> AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
}