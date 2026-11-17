using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IRssFeedDocumentQueryRepository
{
    Task<DateTimeOffset?> FindGeneratedAtAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default);

    Task<(DateTimeOffset GeneratedAt, string Content)?> FindContentAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default);
}
