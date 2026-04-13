using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Tokens;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TokenCommandRepository(DbSet<Token> tokens) :
    CommandRepository<Token, TokenId>(tokens),
    ITokenCommandRepository
{
    public Task DeleteAllAsync(CancellationToken cancellationToken = default) =>
        DeleteAsync(_ => true, cancellationToken);

    Task ITokenCommandRepository.AddAsync(
        Token token,
        CancellationToken cancellationToken) =>
        AddAsync(token, cancellationToken);
}