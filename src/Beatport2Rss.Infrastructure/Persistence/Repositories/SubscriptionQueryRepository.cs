using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionQueryRepository(
    IQueryable<Subscription> subscriptions,
    IQueryable<Tag> tags) :
    ISubscriptionQueryRepository
{
    public Task<bool> ExistsAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default) =>
        subscriptions
            .AnyAsync(
                s =>
                    s.BeatportType == beatportType &&
                    s.BeatportId == beatportId &&
                    s.BeatportSlug == beatportSlug,
                cancellationToken);

    public Task<SubscriptionDetailsReadModel> LoadWithUserTagsAsync(
        UserId userId,
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
                (
                    from subscriptionTag in subscription.Tags
                    join tag in tags
                        on subscriptionTag.TagId equals tag.Id
                    where tag.UserId == userId
                    select new SubscriptionTagDetailsReadModel(tag.Name, tag.Slug)
                ),
                subscription.CreatedAt,
                subscription.RefreshedAt)
        )
        .SingleAsync(cancellationToken);
}