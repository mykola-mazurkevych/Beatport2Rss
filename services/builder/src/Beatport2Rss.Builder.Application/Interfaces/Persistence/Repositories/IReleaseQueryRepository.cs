using Beatport2Rss.Builder.Application.ReadModels.Releases;
using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IReleaseQueryRepository
{
    Task<IReadOnlyCollection<ReleaseReadModel>> GetForFeedAsync(
        FeedId feedId,
        CancellationToken cancellationToken = default);
}
