using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TokenQueryRepository(IQueryable<Token> tokens) :
    ITokenQueryRepository
{
    public Task<Token?> FindAsync(CancellationToken cancellationToken = default) =>
        tokens.SingleOrDefaultAsync(cancellationToken);
}