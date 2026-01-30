using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public readonly record struct UpdateUserStatusCommand(
    Guid UserId,
    bool IsActive) :
    ICommand<Result>;

internal sealed class UpdateUserStatusCommandValidator :
    AbstractValidator<UpdateUserStatusCommand>
{
    public UpdateUserStatusCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

internal sealed class UpdateUserStatusCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userCommandRepository.LoadAsync(userId, cancellationToken).ConfigureAwait(false);

        user.UpdateStatus(command.IsActive);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}