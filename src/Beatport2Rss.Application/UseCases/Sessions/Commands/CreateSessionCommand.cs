using Beatport2Rss.Application.Extensions;
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

public readonly record struct CreateSessionCommand(
    string? EmailAddress,
    string? Password,
    string? UserAgent,
    string? IpAddress) :
    ICommand<Result<CreateSessionResult>>;

public readonly record struct CreateSessionResult(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken);

internal sealed class CreateSessionCommandValidator :
    AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email address is required.")
            .MaximumLength(EmailAddress.MaxLength).WithMessage($"Email address must be at most {EmailAddress.MaxLength} characters.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(Password.MinLength).WithMessage($"Password must be at least {Password.MinLength} characters long.")
            .MaximumLength(Password.MaxLength).WithMessage($"Password must be at most {Password.MaxLength} characters.");

        RuleFor(c => c.UserAgent)
            .MaximumLength(Session.UserAgentMaxLength).WithMessage($"User agent must be at most {Session.UserAgentMaxLength} characters long.");

        // TODO: add validation if this is actually an IP address
        RuleFor(c => c.IpAddress)
            .MaximumLength(Session.IpAddressMaxLength).WithMessage($"IP address must be at most {Session.IpAddressMaxLength} characters long.");
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
    ICommandHandler<CreateSessionCommand, Result<CreateSessionResult>>
{
    public async ValueTask<Result<CreateSessionResult>> Handle(
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

        var result = new CreateSessionResult(
            accessToken.Value,
            accessToken.Type,
            expiresIn,
            refreshToken);

        return result;
    }
}