using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagCommandRepository(DbSet<Tag> tags) :
    CommandRepository<Tag, TagId>(tags),
    ITagCommandRepository
{
    public Task<bool> ExistsAsync(
        UserId userId,
        TagName tagName,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            tag =>
                tag.UserId == userId &&
                tag.Name == tagName,
            cancellationToken);

    public Task<bool> ExistsExceptAsync(
        UserId userId,
        TagName tagName,
        TagId exceptTagId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            tag =>
                tag.UserId == userId &&
                tag.Name == tagName &&
                tag.Id != exceptTagId,
            cancellationToken);

    public Task<Tag> LoadAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            tag =>
                tag.UserId == userId &&
                tag.Slug == slug,
            cancellationToken);

    Task ITagCommandRepository.AddAsync(
        Tag tag,
        CancellationToken cancellationToken) =>
        AddAsync(tag, cancellationToken);
}