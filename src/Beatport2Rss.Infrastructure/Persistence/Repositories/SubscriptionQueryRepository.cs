using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Subscriptions;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionQueryRepository(IQueryable<Subscription> subscriptions) :
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

    public Task<SubscriptionDetailsReadModel> LoadAsync(
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        CancellationToken cancellationToken = default) =>
        subscriptions
            .Where(s =>
                s.BeatportType == beatportType &&
                s.BeatportId == beatportId &&
                s.BeatportSlug == beatportSlug)
            .Select(s => new SubscriptionDetailsReadModel(
                s.Id,
                s.Name,
                s.BeatportType,
                s.BeatportId,
                s.BeatportSlug,
                s.ImageUri,
                s.CreatedAt,
                s.RefreshedAt))
            .SingleAsync(cancellationToken);
}