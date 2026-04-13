using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository
{
    Task<bool> ExistsAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        CancellationToken cancellationToken = default);

    Task<Subscription> LoadWithTagsAsync(
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<Subscription> AddAsync(
        Subscription subscription,
        CancellationToken cancellationToken = default);

    void Update(Subscription subscription);
}