using Beatport2Rss.Api.Application.ReadModels.Tags;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

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