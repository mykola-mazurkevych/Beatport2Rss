using Beatport2Rss.Application.Dtos.Sessions;
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record CreateSessionCommand(
    string? EmailAddress,
    string? Password,
    string? UserAgent,
    string? IpAddress) :
    ICommand<Result<SessionDto>>, IRequireValidation;

internal sealed class CreateSessionCommandValidator :
    AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(c => c.EmailAddress).IsEmailAddress();
        RuleFor(c => c.Password).IsPassword();
        RuleFor(c => c.UserAgent).IsUserAgent();
        RuleFor(c => c.IpAddress).IsIpAddress();
    }
}

internal sealed class CreateSessionCommandHandler(
    IAccessTokenService accessTokenService,
    IClock clock,
    IRefreshTokenService refreshTokenService,
    IPasswordHasher passwordHasher,
    ISessionCommandRepository sessionCommandRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSessionCommand, Result<SessionDto>>
{
    public async ValueTask<Result<SessionDto>> Handle(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = EmailAddress.Create(command.EmailAddress);
        var password = Password.Create(command.Password);
        var userAuthDetails = await userQueryRepository.LoadUserAuthDetailsReadModelAsync(emailAddress, cancellationToken);

        if (userAuthDetails is null || !passwordHasher.Verify(password, userAuthDetails.PasswordHash))
        {
            return Result.Unauthorized("The provided email address or password is incorrect.");
        }

        var sessionId = SessionId.Create(Guid.CreateVersion7());
        (AccessToken accessToken, int expiresIn) = accessTokenService.Generate(userAuthDetails, sessionId);
        (RefreshToken refreshToken, DateTimeOffset expiresAt) = refreshTokenService.Generate();
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);

        var session = Session.Create(
            sessionId,
            clock.UtcNow,
            userAuthDetails.Id,
            refreshTokenHash,
            expiresAt,
            command.UserAgent,
            command.IpAddress);

        await sessionCommandRepository.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SessionDto(
            accessToken,
            accessToken.Type,
            expiresIn,
            refreshToken);
    }
}