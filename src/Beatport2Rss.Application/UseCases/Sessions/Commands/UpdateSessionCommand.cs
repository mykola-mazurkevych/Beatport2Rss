using Beatport2Rss.Application.Dtos.Sessions;
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
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

public sealed record UpdateSessionCommand(
    SessionId SessionId,
    string? RefreshToken) :
    ICommand<Result<SessionDto>>, IRequireValidation;

internal sealed class UpdateSessionCommandValidator :
    AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
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
    ICommandHandler<UpdateSessionCommand, Result<SessionDto>>
{
    public async ValueTask<Result<SessionDto>> Handle(
        UpdateSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionCommandRepository.LoadAsync(command.SessionId, cancellationToken);
        var userAuthDetails = await userQueryRepository.LoadUserAuthDetailsReadModelAsync(session.UserId, cancellationToken);

        var refreshToken = RefreshToken.Create(command.RefreshToken);
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);

        if (session.RefreshTokenExpiresAt < clock.UtcNow || refreshTokenHash != session.RefreshTokenHash)
        {
            sessionCommandRepository.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Unauthorized("The provided refresh token is invalid or has expired.");
        }

        (AccessToken accessToken, int expiresIn) = accessTokenService.Generate(userAuthDetails, command.SessionId);
        (RefreshToken newRefreshToken, DateTimeOffset expiresAt) = refreshTokenService.Generate();

        session.Refresh(refreshTokenService.Hash(newRefreshToken), expiresAt);

        sessionCommandRepository.Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SessionDto(
            accessToken,
            accessToken.Type,
            expiresIn,
            newRefreshToken);
    }
}