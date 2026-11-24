using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record UpdateUserStatusCommand(
    Guid UserId,
    bool IsActive,
    string? ConfirmedEmailAddress = null) : ICommand<Result>;

internal sealed class UpdateUserStatusCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserStatusCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        if (command.ConfirmedEmailAddress is { } emailAddress)
        {
            if (!command.IsActive)
            {
                throw new InvalidOperationException("An email confirmation cannot deactivate a user.");
            }

            await userCommandRepository.ActivatePendingAsync(userId, EmailAddress.Create(emailAddress), cancellationToken);
            return Result.Ok();
        }

        var user = await userCommandRepository.FindAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("The user was not found.");
        }

        var status = command.IsActive ? UserStatus.Active : UserStatus.Inactive;
        if (user.Status != status)
        {
            user.UpdateStatus(command.IsActive);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok();
    }
}
