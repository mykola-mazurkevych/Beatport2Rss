using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record UpdateSessionRequest(
    string? RefreshToken);

public sealed record UpdateSessionResponse(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken);

public sealed record UpdateSessionCommand(
    Guid SessionId,
    string? RefreshToken) :
    ICommand<Result<UpdateSessionResponse>>;

internal sealed class UpdateSessionCommandValidator :
    AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(c => c.SessionId).IsSessionId();
        RuleFor(c => c.RefreshToken).IsRefreshToken();
    }
}

internal sealed class UpdateSessionCommandHandler(
    IClock clock,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    ISessionCommandRepository sessionCommandRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateSessionCommand, Result<UpdateSessionResponse>>
{
    public async ValueTask<Result<UpdateSessionResponse>> Handle(
        UpdateSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var sessionId = SessionId.Create(command.SessionId);
        var session = await sessionCommandRepository.LoadAsync(sessionId, cancellationToken);
        var userAuthDetails = await userQueryRepository.LoadUserAuthDetailsReadModelAsync(session.UserId, cancellationToken);

        var refreshToken = RefreshToken.Create(command.RefreshToken);
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);

        if (session.RefreshTokenExpiresAt < clock.UtcNow || refreshTokenHash != session.RefreshTokenHash)
        {
            sessionCommandRepository.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Unauthorized("The provided refresh token is invalid or has expired.");
        }

        (AccessToken accessToken, int expiresIn) = accessTokenService.Generate(userAuthDetails, sessionId);
        (RefreshToken newRefreshToken, DateTimeOffset expiresAt) = refreshTokenService.Generate();

        session.Refresh(refreshTokenService.Hash(newRefreshToken), expiresAt);

        sessionCommandRepository.Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateSessionResponse(
            accessToken.Value,
            accessToken.Type,
            expiresIn,
            newRefreshToken);
    }
}