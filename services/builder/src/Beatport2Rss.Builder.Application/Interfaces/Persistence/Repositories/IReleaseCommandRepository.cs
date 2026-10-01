using System.Linq.Expressions;

using Beatport2Rss.Builder.Domain.Releases;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IReleaseCommandRepository
{
    Task DeleteAsync(
        Expression<Func<Release, bool>> predicate,
        CancellationToken cancellationToken = default);
}