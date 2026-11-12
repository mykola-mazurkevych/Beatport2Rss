using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class ReleaseCommandRepository(DbSet<Release> releases) :
    CommandRepository<Release>(releases),
    IReleaseCommandRepository
{
    public Task<IEnumerable<Release>> FindAllAsync(
        IEnumerable<BeatportId> beatportIds,
        CancellationToken cancellationToken = default) =>
        FindAllAsync(release => beatportIds.Contains(release.BeatportId), cancellationToken);
}