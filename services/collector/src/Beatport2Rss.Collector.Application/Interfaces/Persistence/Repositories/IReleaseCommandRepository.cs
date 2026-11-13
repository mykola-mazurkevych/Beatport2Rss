using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

public interface IReleaseCommandRepository
{
    Task<IEnumerable<Release>> FindAllAsync(IEnumerable<BeatportId> beatportIds, CancellationToken cancellationToken = default);

    Task<Release> AddAsync(Release release, CancellationToken cancellationToken = default);
}