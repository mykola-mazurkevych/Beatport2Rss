using Beatport2Rss.Domain.Tokens;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITokenCommandRepository
{
    Task AddAsync(
        Token token,
        CancellationToken cancellationToken = default);

    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}