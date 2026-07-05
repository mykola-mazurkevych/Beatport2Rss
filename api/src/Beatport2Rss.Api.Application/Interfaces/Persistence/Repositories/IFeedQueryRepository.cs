using Beatport2Rss.Api.Application.ReadModels.Feeds;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface IFeedQueryRepository
{
    IQueryable<FeedPaginableReadModel> GetFeedPaginableReadModelsAsQueryable(UserId userId);

    Task<bool> ExistsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<FeedDetailsReadModel> LoadFeedDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);
}