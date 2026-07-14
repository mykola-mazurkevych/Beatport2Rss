using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> subscriptions) :
    CommandRepository<Subscription, SubscriptionId>(subscriptions),
    ISubscriptionCommandRepository
{
    private readonly DbSet<Subscription> _subscriptions = subscriptions;

    public Task<bool> ExistsAsync(
        SubscriptionType type,
        BeatportId beatportId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            subscription =>
                subscription.Type == type &&
                subscription.BeatportId == beatportId,
            cancellationToken);

    public Task<Subscription> LoadWithTagsAsync(
        Slug slug,
        CancellationToken cancellationToken = default) =>
        _subscriptions
            .Include(subscription => subscription.Tags)
            .SingleAsync(
                subscription => subscription.Slug == slug,
                cancellationToken);
}