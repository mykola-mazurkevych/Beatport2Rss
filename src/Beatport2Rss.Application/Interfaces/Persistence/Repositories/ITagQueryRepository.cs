using Beatport2Rss.Application.Interfaces.Models.Tags;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITagQueryRepository
{
    IQueryable<Tag> Tags { get; } // TODO: Delete

    Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);

    Task<TagId> LoadTagIdAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);
    Task<IHaveTagDetails> LoadTagDetailsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);
}