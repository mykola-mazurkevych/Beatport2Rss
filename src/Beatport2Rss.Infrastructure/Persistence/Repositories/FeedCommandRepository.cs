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

    public Task<bool> ExistsAsync(
        UserId userId,
        FeedName feedName,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            feed =>
                feed.UserId == userId &&
                feed.Name == feedName,
            cancellationToken);

    public Task<bool> ExistsExceptAsync(
        UserId userId,
        FeedName feedName,
        FeedId exceptFeedId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            feed =>
                feed.UserId == userId &&
                feed.Name == feedName &&
                feed.Id != exceptFeedId,
            cancellationToken);

    public Task<Feed> LoadAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            feed =>
                feed.UserId == userId &&
                feed.Slug == slug,
            cancellationToken);

    public Task<Feed> LoadWithSubscriptionsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        _feeds
            .Include(feed => feed.Subscriptions)
            .SingleAsync(
                feed =>
                    feed.UserId == userId &&
                    feed.Slug == slug,
                cancellationToken);

    Task IFeedCommandRepository.AddAsync(
        Feed feed,
        CancellationToken cancellationToken) =>
        AddAsync(feed, cancellationToken);
}