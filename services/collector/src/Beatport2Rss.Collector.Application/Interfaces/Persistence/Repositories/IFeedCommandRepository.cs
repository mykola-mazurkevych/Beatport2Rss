using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

public interface IFeedCommandRepository
{
    Task<bool> ExistsAsync(FeedId feedId, CancellationToken cancellationToken = default);

    Task<Feed?> FindAsync(FeedId feedId, CancellationToken cancellationToken = default);

    Task<Feed> AddAsync(Feed feed, CancellationToken cancellationToken = default);
    void Delete(Feed feed);
}