using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Feeds;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class FeedQueryRepository(
    IQueryable<Feed> feeds,
    IQueryable<FeedQueryModel> feedQueryModels) :
    QueryRepository<FeedQueryModel, FeedId>(feedQueryModels),
    IFeedQueryRepository
{
    public IQueryable<Feed> Feeds => feeds;

    public Task<bool> ExistsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            feedQueryModel =>
                feedQueryModel.UserId == userId &&
                feedQueryModel.Slug == slug,
            cancellationToken);

    public async Task<IHaveFeedDetails> LoadFeedDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            feedQueryModel =>
                feedQueryModel.UserId == userId &&
                feedQueryModel.Slug == slug,
            FeedDetails.Selector,
            cancellationToken);

    private sealed record FeedDetails(
        FeedId Id,
        FeedName Name,
        Slug Slug,
        bool IsActive,
        DateTimeOffset CreatedAt,
        int SubscriptionsCount) :
        IHaveFeedDetails
    {
        public static Expression<Func<FeedQueryModel, FeedDetails>> Selector =>
            feedQueryModel => new FeedDetails(
                feedQueryModel.Id,
                feedQueryModel.Name,
                feedQueryModel.Slug,
                feedQueryModel.IsActive,
                feedQueryModel.CreatedAt,
                feedQueryModel.SubscriptionsCount);
    }
}