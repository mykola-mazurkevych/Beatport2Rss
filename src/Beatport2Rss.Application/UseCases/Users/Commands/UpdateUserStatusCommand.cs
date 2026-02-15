using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public sealed record UpdateUserStatusCommand(
    UserId UserId,
    bool IsActive) :
    ICommand<Result>;

internal sealed class UpdateUserStatusCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken).ConfigureAwait(false);

        user.UpdateStatus(command.IsActive);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}