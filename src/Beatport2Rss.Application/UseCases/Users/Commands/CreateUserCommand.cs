using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Checkers;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Interfaces.Services.Security;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Users;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public readonly record struct CreateUserCommand(
    string? EmailAddress,
    string? Password,
    string? FirstName,
    string? LastName);

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.EmailAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email address is required.")
            .MaximumLength(EmailAddress.MaxLength).WithMessage($"Email address must be at most {EmailAddress.MaxLength} characters long.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(c => c.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(Password.MinLength).WithMessage($"Password must be at least {Password.MinLength} characters long.")
            .MaximumLength(Password.MaxLength).WithMessage($"Password must be at most {Password.MaxLength} characters long.");

        RuleFor(c => c.FirstName)
            .MaximumLength(User.NameLength).WithMessage($"First name must be at most {User.NameLength} characters long.");
        
        RuleFor(c => c.LastName)
            .MaximumLength(User.NameLength).WithMessage($"Last name must be at most {User.NameLength} characters long.");
    }
}

public sealed class CreateUserCommandHandler(
    IValidator<CreateUserCommand> validator,
    IClock clock,
    IEmailAddressAvailabilityChecker emailAddressAvailabilityChecker,
    IPasswordHasher passwordHasher,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Created, ValidationFailed, EmailAddressAlreadyTaken>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var emailAddress = EmailAddress.Create(command.EmailAddress);

        if (!await emailAddressAvailabilityChecker.IsAvailableAsync(emailAddress, cancellationToken))
        {
            return new EmailAddressAlreadyTaken(emailAddress);
        }

        var userId = UserId.Create(Guid.CreateVersion7());
        var password = Password.Create(command.Password);
        var passwordHash = passwordHasher.Hash(password);

        var user = User.Create(
            userId,
            emailAddress,
            passwordHash,
            command.FirstName,
            command.LastName,
            UserStatus.Pending,
            clock.UtcNow);

        await userCommandRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Created();
    }
}