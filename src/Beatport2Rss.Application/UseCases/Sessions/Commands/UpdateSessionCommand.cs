using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Results;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct UpdateSessionCommand(
    Guid SessionId,
    string RefreshToken);

public readonly record struct UpdateSessionResult(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken);

public sealed class UpdateSessionCommandValidator :
    AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
        
        RuleFor(c => c.RefreshToken)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Refresh token is required.")
            .Length(RefreshToken.Length).WithMessage($"Refresh token must be exactly {RefreshToken.Length} characters long.");
    }
}

public sealed class UpdateSessionCommandHandler(
    IValidator<UpdateSessionCommand> validator,
    IClock clock,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    ISessionCommandRepository sessionCommandRepository,
    ISessionQueryRepository sessionQueryRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Success<UpdateSessionResult>, ValidationFailed, Unauthorized, InactiveUser>> HandleAsync(
        UpdateSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var sessionId = SessionId.Create(command.SessionId);
        var session = (await sessionQueryRepository.GetAsync(sessionId, cancellationToken))!;
        var user = (await userQueryRepository.GetAsync(session.UserId, cancellationToken))!;

        if (!user.IsActive)
        {
            return new InactiveUser();
        }

        var refreshToken = RefreshToken.Create(command.RefreshToken);
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);

        if (session.RefreshTokenExpiresAt < clock.UtcNow || refreshTokenHash != session.RefreshTokenHash)
        {
            sessionCommandRepository.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new Unauthorized("Refresh token is not valid.");
        }

        (AccessToken accessToken, int expiresIn) = accessTokenService.Generate(user, sessionId);
        (RefreshToken newRefreshToken, DateTimeOffset expiresAt) = refreshTokenService.Generate();

        session.Refresh(refreshTokenService.Hash(newRefreshToken), expiresAt);

        sessionCommandRepository.Update(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new UpdateSessionResult(
            accessToken.Value,
            accessToken.Type,
            expiresIn,
            newRefreshToken);

        return new Success<UpdateSessionResult>(result);
    }
}