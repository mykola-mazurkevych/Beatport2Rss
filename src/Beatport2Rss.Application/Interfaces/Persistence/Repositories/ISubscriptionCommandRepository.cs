using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository :
    ICommandRepository<Subscription, SubscriptionId>
{
    Task<Subscription> LoadWithTagsAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default);
}