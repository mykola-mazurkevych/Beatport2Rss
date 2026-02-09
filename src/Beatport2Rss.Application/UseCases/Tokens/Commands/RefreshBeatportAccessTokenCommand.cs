#pragma warning disable CA1031 // Do not catch general exception types

using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Tokens;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tokens.Commands;

public sealed record RefreshBeatportAccessTokenCommand :
    ICommand<Result<BeatportAccessToken>>;

internal sealed class RefreshBeatportAccessTokenCommandHandler(
    IClock clock,
    ITokenCommandRepository tokenCommandRepository,
    IUnitOfWork unitOfWork,
    IBeatportAccessTokenProvider beatportAccessTokenProvider) :
    ICommandHandler<RefreshBeatportAccessTokenCommand, Result<BeatportAccessToken>>
{
    public async ValueTask<Result<BeatportAccessToken>> Handle(
        RefreshBeatportAccessTokenCommand command,
        CancellationToken cancellationToken)
    {
        string? accessToken;
        int expiresIn;

        try
        {
            (accessToken, expiresIn) = await beatportAccessTokenProvider.ProvideAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }

        var tokens = await tokenCommandRepository.FindAllAsync(_ => true, cancellationToken);

        var tokenId = TokenId.Create(Guid.NewGuid());
        var beatportAccessToken = BeatportAccessToken.Create(accessToken);
        var token = Token.Create(
            tokenId,
            clock.UtcNow,
            beatportAccessToken,
            clock.UtcNow.AddSeconds(expiresIn));

        tokenCommandRepository.DeleteRange(tokens);
        await tokenCommandRepository.AddAsync(token, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return beatportAccessToken;
    }
}