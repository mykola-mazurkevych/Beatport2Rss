using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Tags;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagQueryRepository(
    IQueryable<Tag> tags,
    IQueryable<TagQueryModel> tagQueryModels) :
    QueryRepository<TagQueryModel, TagId>(tagQueryModels),
    ITagQueryRepository
{
    public IQueryable<Tag> Tags => tags;

    public Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        base.ExistsAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            cancellationToken);

    public Task<TagId> LoadTagIdAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        LoadAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            tagQueryModel => tagQueryModel.Id,
            cancellationToken);

    public async Task<IHaveTagDetails> LoadTagDetailsAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            tagQueryModel =>
                tagQueryModel.UserId == userId &&
                tagQueryModel.Slug == slug,
            TagDetails.Selector,
            cancellationToken);

    private sealed record TagDetails(
        TagId Id,
        TagName Name,
        Slug Slug,
        DateTimeOffset CreatedAt) :
        IHaveTagDetails
    {
        public static Expression<Func<TagQueryModel, TagDetails>> Selector =>
            tagQueryModel => new TagDetails(
                tagQueryModel.Id,
                tagQueryModel.Name,
                tagQueryModel.Slug,
                tagQueryModel.CreatedAt);
    }
}