using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITagQueryRepository :
    IQueryRepository
{
    Task<bool> ExistsAsync(UserId userId, Slug slug, CancellationToken cancellationToken = default);
}