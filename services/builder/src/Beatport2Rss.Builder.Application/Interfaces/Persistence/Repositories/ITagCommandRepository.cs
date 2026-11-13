using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface ITagCommandRepository
{
    Task<bool> ExistsAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    Task<Tag?> FindAsync(
        TagId tagId,
        CancellationToken cancellationToken = default);

    Task<Tag> AddAsync(
        Tag tag,
        CancellationToken cancellationToken = default);

    void Delete(Tag tag);
}