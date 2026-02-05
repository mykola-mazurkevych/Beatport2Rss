using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) :
    ICommand<Result>, IRequireActiveUser;

internal sealed class DeleteUserCommandValidator :
    AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(c => c.UserId).IsUserId();
    }
}

internal sealed class DeleteUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userCommandRepository.LoadAsync(userId, cancellationToken);

        userCommandRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}