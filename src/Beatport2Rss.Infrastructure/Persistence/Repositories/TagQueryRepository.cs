using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Tags;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagQueryRepository(IQueryable<Tag> tags) :
    ITagQueryRepository
{
    public IQueryable<Tag> Tags => tags;

    public Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        tags.AnyAsync(t => t.UserId == userId && t.Slug == slug, cancellationToken);

    public Task<TagId> LoadTagIdAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        (
            from tag in tags
            where tag.UserId == userId &&
                  tag.Slug == slug
            select tag.Id
        )
        .SingleAsync(cancellationToken);


    public Task<TagDetailsReadModel> LoadTagDetailsReadModelAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default) =>
        (
            from tag in tags
            where tag.UserId == userId &&
                  tag.Slug == slug
            select new TagDetailsReadModel(
                tag.Id,
                tag.Name,
                tag.Slug,
                tag.CreatedAt)
        )
        .SingleAsync(cancellationToken);
}