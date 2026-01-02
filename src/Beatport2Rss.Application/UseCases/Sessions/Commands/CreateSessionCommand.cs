using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using OneOf;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct CreateSessionCommand(
    string? EmailAddress,
    string? Password,
    string? UserAgent,
    string? IpAddress);

public readonly record struct CreateSessionResult(
    string AccessToken,
    AccessTokenType TokenType,
    int ExpiresIn,
    string RefreshToken);

public sealed class CreateSessionCommandValidator :
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

public sealed class CreateSessionCommandHandler(
    IValidator<CreateSessionCommand> validator,
    IAccessTokenService accessTokenService,
    IClock clock,
    IRefreshTokenService refreshTokenService,
    IPasswordHasher passwordHasher,
    ISessionCommandRepository sessionCommandRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Success<CreateSessionResult>, ValidationFailed, InvalidCredentials, InactiveUser>> HandleAsync(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var emailAddress = EmailAddress.Create(command.EmailAddress);
        var password = Password.Create(command.Password);
        var user = await userQueryRepository.GetAsync(u => u.EmailAddress == emailAddress, cancellationToken);

        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
        {
            return new InvalidCredentials();
        }

        if (!user.IsActive)
        {
            return new InactiveUser();
        }

        var sessionId = SessionId.Create(Guid.CreateVersion7());
        (AccessToken accessToken, int expiresIn) = accessTokenService.Generate(user, sessionId);
        (RefreshToken refreshToken, DateTimeOffset expiresAt) = refreshTokenService.Generate();
        var refreshTokenHash = refreshTokenService.Hash(refreshToken);

        var session = Session.Create(
            sessionId,
            user.Id,
            clock.UtcNow,
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

        return new Success<CreateSessionResult>(result);
    }
}