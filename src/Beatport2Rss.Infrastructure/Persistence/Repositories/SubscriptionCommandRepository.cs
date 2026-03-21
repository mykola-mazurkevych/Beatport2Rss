using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> subscriptions) :
    CommandRepository<Subscription, SubscriptionId>(subscriptions),
    ISubscriptionCommandRepository
{
    private readonly DbSet<Subscription> _subscriptions = subscriptions;

    public Task<Subscription> LoadWithTagsAsync(
        Slug slug,
        CancellationToken cancellationToken = default) =>
        _subscriptions
            .Include(s => s.Tags)
            .SingleAsync(s => s.Slug == slug, cancellationToken);
}