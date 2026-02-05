using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public sealed record CreateUserRequest(
    string? EmailAddress,
    string? Password,
    string? FirstName,
    string? LastName);

public sealed record CreateUserCommand(
    string? EmailAddress,
    string? Password,
    string? FirstName,
    string? LastName) :
    ICommand<Result>;

internal sealed class CreateUserCommandValidator :
    AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.EmailAddress).IsEmailAddress();
        RuleFor(c => c.Password).IsPassword();
        RuleFor(c => c.FirstName).IsFirstName();
        RuleFor(c => c.LastName).IsLastName();
    }
}

internal sealed class CreateUserCommandHandler(
    IClock clock,
    IPasswordHasher passwordHasher,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var emailAddress = EmailAddress.Create(command.EmailAddress);

        var existingUser = await userCommandRepository.FindAsync(u => u.EmailAddress == emailAddress, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Conflict($"Email address {emailAddress} already taken.");
        }

        var userId = UserId.Create(Guid.CreateVersion7());
        var password = Password.Create(command.Password);
        var passwordHash = passwordHasher.Hash(password);

        var user = User.Create(
            userId,
            clock.UtcNow,
            emailAddress,
            passwordHash,
            command.FirstName,
            command.LastName,
            UserStatus.Pending);

        await userCommandRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}