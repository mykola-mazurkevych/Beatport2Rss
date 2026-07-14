using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository
{
    Task<bool> ExistsAsync(
        SubscriptionType type,
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