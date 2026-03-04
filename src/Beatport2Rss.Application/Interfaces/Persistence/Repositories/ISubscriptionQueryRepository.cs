using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionQueryRepository :
    IQueryRepository
{
    Task<SubscriptionDetailsReadModel?> LoadAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default);
}