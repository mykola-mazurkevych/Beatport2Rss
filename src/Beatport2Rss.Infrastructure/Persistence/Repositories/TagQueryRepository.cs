using Beatport2Rss.Application.ReadModels.Tags;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagQueryRepository(
    IQueryable<TagQueryModel> tagQueryModels) :
    QueryRepository<TagQueryModel, TagId>(tagQueryModels),
    ITagQueryRepository
{
    private readonly IQueryable<TagQueryModel> _tagQueryModels = tagQueryModels;

    public IQueryable<TagPaginableReadModel> GetTagPaginableReadModelsAsQueryable(UserId userId) =>
        _tagQueryModels
            .Where(tagQueryModel => tagQueryModel.UserId == userId)
            .Select(tagQueryModel => new TagPaginableReadModel
            {
                Id = tagQueryModel.Id,
                CreatedAt = tagQueryModel.CreatedAt,
                Name = tagQueryModel.Name,
                Slug = tagQueryModel.Slug
            });

    public Task<bool> ExistsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            cancellationToken);

    public Task<TagId> LoadTagIdAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            tagQueryModel => tagQueryModel.Id,
            cancellationToken);

    public async Task<TagDetailsReadModel> LoadTagDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            tagQueryModel => new TagDetailsReadModel(
                tagQueryModel.Id,
                tagQueryModel.Name,
                tagQueryModel.Slug,
                tagQueryModel.CreatedAt),
            cancellationToken);
}