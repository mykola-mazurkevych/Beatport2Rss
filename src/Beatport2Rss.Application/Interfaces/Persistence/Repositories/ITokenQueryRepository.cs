using Beatport2Rss.Domain.Tokens;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITokenQueryRepository
{
    Task<Token?> FindAsync(CancellationToken cancellationToken = default);
}