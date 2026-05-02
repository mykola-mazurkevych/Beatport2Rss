using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tokens.Queries;

public sealed record GetUnexpiredTokenQuery :
    IQuery<Result<Token>>;

internal sealed class GetUnexpiredTokenQueryHandler(
    IClock clock,
    ITokenQueryRepository tokenQueryRepository) :
    IQueryHandler<GetUnexpiredTokenQuery, Result<Token>>
{
    public async ValueTask<Result<Token>> Handle(
        GetUnexpiredTokenQuery query,
        CancellationToken cancellationToken)
    {
        var token = await tokenQueryRepository.FindAsync(cancellationToken);

        return token is null || token.ExpiresAt < clock.UtcNow
            ? Result.NotFound("Unexpired token not found.")
            : token;
    }
}