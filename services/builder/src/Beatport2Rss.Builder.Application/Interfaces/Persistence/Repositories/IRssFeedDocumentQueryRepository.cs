using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IRssFeedDocumentQueryRepository
{
    Task<DateTimeOffset> GetGeneratedAtAsync(Slug slug, CancellationToken cancellationToken = default);
    Task<string> GetContentAsync(Slug slug, CancellationToken cancellationToken = default);
}