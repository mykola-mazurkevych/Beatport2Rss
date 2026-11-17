using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record DeleteUserCommand(
    UserId UserId) :
    ICommand<Result>;

internal sealed class DeleteUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IClock clock,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        userCommandRepository.Delete(user);
        var userDeleted = new UserDeleted(
            Id: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            user.Id.Value);
        await integrationEventOutbox.EnqueueAsync(userDeleted, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}