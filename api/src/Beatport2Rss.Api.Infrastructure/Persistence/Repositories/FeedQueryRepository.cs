using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.ReadModels.Feeds;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class FeedQueryRepository(
    IQueryable<FeedQueryModel> feedQueryModels) :
    QueryRepository<FeedQueryModel, FeedId>(feedQueryModels),
    IFeedQueryRepository
{
    private readonly IQueryable<FeedQueryModel> _feedQueryModels = feedQueryModels;

    public IQueryable<FeedPaginableReadModel> GetFeedPaginableReadModelsAsQueryable(UserId userId) =>
        _feedQueryModels
            .Where(feedQueryModel => feedQueryModel.UserId == userId)
            .Select(feedQueryModel => new FeedPaginableReadModel
            {
                Id = feedQueryModel.Id,
                CreatedAt = feedQueryModel.CreatedAt,
                Name = feedQueryModel.Name,
                Slug = feedQueryModel.Slug,
                IsActive = feedQueryModel.IsActive,
                SubscriptionsCount = feedQueryModel.SubscriptionsCount
            });

    public Task<bool> ExistsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            feedQueryModel =>
                feedQueryModel.UserId == userId &&
                feedQueryModel.Slug == slug,
            cancellationToken);

    public async Task<FeedDetailsReadModel> LoadFeedDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            feedQueryModel =>
                feedQueryModel.UserId == userId &&
                feedQueryModel.Slug == slug,
            feedQueryModel => new FeedDetailsReadModel(
                feedQueryModel.Id,
                feedQueryModel.Name,
                feedQueryModel.Slug,
                feedQueryModel.IsActive,
                feedQueryModel.CreatedAt,
                feedQueryModel.SubscriptionsCount),
            cancellationToken);
}