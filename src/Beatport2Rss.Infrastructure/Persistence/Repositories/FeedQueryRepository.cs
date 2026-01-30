using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Feeds;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedQueryRepository(Beatport2RssDbContext dbContext) :
    IFeedQueryRepository
{
    public Task<FeedDetailsReadModel> LoadFeedDetailsQueryModelAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        GetFeedDetailsReadModelsAsQueryable(userId, slug).SingleAsync(cancellationToken);

    private IQueryable<FeedDetailsReadModel> GetFeedDetailsReadModelsAsQueryable(UserId userId, Slug slug) =>
        from feed in dbContext.Feeds
        join user in dbContext.Users on feed.UserId equals user.Id
        where feed.UserId == userId &&
              feed.Slug == slug
        select new FeedDetailsReadModel(
            feed.Id,
            feed.CreatedAt,
            user.FullName,
            feed.Name,
            feed.Slug,
            feed.Status == FeedStatus.Active);
}