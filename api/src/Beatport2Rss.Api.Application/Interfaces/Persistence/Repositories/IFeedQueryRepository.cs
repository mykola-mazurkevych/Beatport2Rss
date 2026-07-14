using Beatport2Rss.Api.Application.ReadModels.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

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