using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface ITagCommandRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        TagName tagName,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsExceptAsync(
        UserId userId,
        TagName tagName,
        TagId exceptTagId,
        CancellationToken cancellationToken = default);

    Task<Tag> LoadAsync(
        UserId userId,
        Slug slug,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Tag tag,
        CancellationToken cancellationToken = default);

    void Update(Tag tag);

    void Delete(Tag tag);
}