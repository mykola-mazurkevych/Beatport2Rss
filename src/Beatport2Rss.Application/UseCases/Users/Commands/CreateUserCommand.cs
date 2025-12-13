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
            .MinimumLength(PasswordHash.PasswordMinLength).WithMessage("Password must be at least 8 characters long.");
    }
}

public sealed class CreateUserCommandHandler(
    ISlugGenerator slugGenerator,
    ICommandRepository<User, UserId> userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = User.Create(
            UserId.Create(Guid.CreateVersion7()),
            Username.Create(command.Username),
            slugGenerator.Generate(command.Username),
            PasswordHash.Create(command.Password),
            EmailAddress.Create(command.EmailAddress),
            DateTimeOffset.UtcNow);

        await userCommandRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}