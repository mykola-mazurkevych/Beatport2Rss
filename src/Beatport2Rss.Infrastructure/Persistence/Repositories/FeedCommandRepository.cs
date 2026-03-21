using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedCommandRepository(DbSet<Feed> feeds) :
    CommandRepository<Feed, FeedId>(feeds),
    IFeedCommandRepository
{
    private readonly DbSet<Feed> _feeds = feeds;

    public Task<Feed> LoadWithSubscriptionsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        _feeds
            .Include(f => f.Subscriptions)
            .SingleAsync(
                f =>
                    f.UserId == userId &&
                    f.Slug == slug,
                cancellationToken);
}