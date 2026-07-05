using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

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