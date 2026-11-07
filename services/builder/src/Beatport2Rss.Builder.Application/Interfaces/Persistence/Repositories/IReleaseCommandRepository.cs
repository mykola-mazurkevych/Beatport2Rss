using System.Linq.Expressions;

using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Common.Domain.Releases;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IReleaseCommandRepository
{
    Task<bool> ExistsAsync(ReleaseId releaseId, CancellationToken cancellationToken = default);

    Task<Release> AddAsync(Release release, CancellationToken cancellationToken = default);
    Task DeleteAsync(Expression<Func<Release, bool>> predicate, CancellationToken cancellationToken = default);
}