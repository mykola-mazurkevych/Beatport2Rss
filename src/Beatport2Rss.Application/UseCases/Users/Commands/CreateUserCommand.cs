using Beatport2Rss.Contracts.Interfaces;
using Beatport2Rss.Contracts.Persistence;
using Beatport2Rss.Contracts.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public readonly record struct CreateUserCommand(
    string? Username,
    string? EmailAddress,
    string? Password);

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IQueryRepository<User, UserId> userQueryRepository)
    {
        RuleFor(c => c.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(Username.MaxLength).WithMessage($"Username must be at most {Username.MaxLength} characters.")
            .Matches(Username.RegexPattern).WithMessage("Username contains invalid characters.")
            .MustAsync((username, cancellationToken) => userQueryRepository.NotExistsAsync(u => u.Username == username, cancellationToken)).WithMessage("Username is already taken.");

        RuleFor(c => c.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .MaximumLength(EmailAddress.MaxLength).WithMessage($"Email address must be at most {EmailAddress.MaxLength} characters.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MustAsync((emailAddress, cancellationToken) => userQueryRepository.NotExistsAsync(u => u.EmailAddress == emailAddress, cancellationToken)).WithMessage("Email address is already taken.");

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(Password.MinLength).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(Password.MaxLength).WithMessage($"Password must be at most {Password.MaxLength} characters.");
    }
}

public sealed class CreateUserCommandHandler(
    IPasswordHasher passwordHasher,
    ISlugGenerator slugGenerator,
    ICommandRepository<User, UserId> userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var username = Username.Create(command.Username);
        var password = Password.Create(command.Password);

        var user = User.Create(
            UserId.Create(Guid.CreateVersion7()),
            username,
            slugGenerator.Generate(username),
            passwordHasher.Hash(password),
            EmailAddress.Create(command.EmailAddress),
            DateTimeOffset.UtcNow);

        await userCommandRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}