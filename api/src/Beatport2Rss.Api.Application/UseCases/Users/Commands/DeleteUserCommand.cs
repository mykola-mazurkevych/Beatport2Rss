using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record DeleteUserCommand(
    UserId UserId) :
    ICommand<Result>;

internal sealed class DeleteUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        userCommandRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}