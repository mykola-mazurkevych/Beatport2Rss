using Beatport2Rss.Builder.Domain.Feeds;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IFeedCommandRepository
{
    Task<bool> ExistsAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default);

    Task<Feed?> FindAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default);

    Task<Feed> AddAsync(
        Feed feed,
        CancellationToken cancellationToken = default);

    void Delete(Feed feed);
}