using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionQueryRepository :
    IQueryRepository
{
    Task<bool> ExistsAsync(BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default);

    Task<SubscriptionDetailsReadModel> LoadWithUserTagsAsync(
        UserId userId,
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default);
}