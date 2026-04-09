using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedQueryRepository(
    IQueryable<Feed> feeds) :
    IFeedQueryRepository
{
    public IQueryable<Feed> Feeds => feeds;

    public Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        feeds.AnyAsync(
            f =>
                f.UserId == userId &&
                f.Slug == slug,
            cancellationToken);

    public Task<FeedDetailsReadModel> LoadFeedDetailsReadModelAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        (
            from feed in feeds
            where feed.UserId == userId &&
                  feed.Slug == slug
            select new FeedDetailsReadModel(
                feed.Id,
                feed.Slug,
                feed.Name,
                feed.IsActive,
                feed.CreatedAt,
                feed.Subscriptions.Count)
        )
        .SingleAsync(cancellationToken);
}