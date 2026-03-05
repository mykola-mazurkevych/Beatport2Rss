using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionQueryRepository(IQueryable<Subscription> subscriptions) :
    ISubscriptionQueryRepository
{
    public Task<SubscriptionDetailsReadModel?> LoadAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default) =>
        (
            from subscription in subscriptions
            where subscription.BeatportType == beatportType &&
                  subscription.BeatportId == beatportId &&
                  subscription.BeatportSlug == beatportSlug
            select new SubscriptionDetailsReadModel(
                subscription.Id,
                subscription.Name,
                subscription.BeatportType,
                subscription.BeatportId,
                subscription.BeatportSlug,
                subscription.ImageUri,
                subscription.CreatedAt,
                subscription.RefreshedAt)
        )
        .SingleOrDefaultAsync(cancellationToken);
}