using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Users;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionsCommand(Guid UserId);

public sealed class DeleteSessionsCommandValidator :
    AbstractValidator<DeleteSessionsCommand>
{
    public DeleteSessionsCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

public sealed class DeleteSessionsCommandHandler(
    IValidator<DeleteSessionsCommand> validator,
    ISessionCommandRepository sessionCommandRepository,
    ISessionQueryRepository sessionQueryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Success, ValidationFailed>> HandleAsync(DeleteSessionsCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var userId = UserId.Create(command.UserId);
        var sessions = await sessionQueryRepository.GetSessionsAsync(userId, cancellationToken);

        sessionCommandRepository.DeleteRange(sessions);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}