using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public readonly record struct CreateUserCommand(
    string? EmailAddress,
    string? Password);

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IEmailAddressAvailabilityChecker emailAddressAvailabilityChecker)
    {
        RuleFor(c => c.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email address is required.")
            .MaximumLength(EmailAddress.MaxLength).WithMessage($"Email address must be at most {EmailAddress.MaxLength} characters.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MustAsync(emailAddressAvailabilityChecker.IsAvailableAsync).WithMessage("Email address is already taken.");

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(Password.MinLength).WithMessage($"Password must be at least {Password.MinLength} characters long.")
            .MaximumLength(Password.MaxLength).WithMessage($"Password must be at most {Password.MaxLength} characters.");
    }
}

public sealed class CreateUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = User.Create(
            UserId.Create(Guid.CreateVersion7()),
            EmailAddress.Create(command.EmailAddress),
            passwordHasher.Hash(Password.Create(command.Password)),
            null,
            null,
            UserStatus.Pending,
            DateTimeOffset.UtcNow);

        await userCommandRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}