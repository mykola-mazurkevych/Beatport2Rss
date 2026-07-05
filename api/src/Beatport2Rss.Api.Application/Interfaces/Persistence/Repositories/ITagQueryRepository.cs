using Beatport2Rss.Api.Application.ReadModels.Tags;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface ITagQueryRepository
{
    IQueryable<TagPaginableReadModel> GetTagPaginableReadModelsAsQueryable(UserId userId);

    Task<bool> ExistsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<TagId> LoadTagIdAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task<TagDetailsReadModel> LoadTagDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);
}