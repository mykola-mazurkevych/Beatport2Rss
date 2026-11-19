using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record ActivateUserCommand(
    UserId UserId,
    UserId CallerUserId) :
    ICommand<Result>;

internal sealed class ActivateUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork,
    IOptions<EmailConfirmationOptions> emailConfirmationOptions) :
    ICommandHandler<ActivateUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        ActivateUserCommand command,
        CancellationToken cancellationToken)
    {
        var options = emailConfirmationOptions.Value;
        if (options.Enabled ||
            options.AdministratorUserId is not { } administratorUserId ||
            administratorUserId == Guid.Empty ||
            administratorUserId != command.CallerUserId.Value ||
            command.CallerUserId == command.UserId ||
            !await userQueryRepository.ExistsAsync(command.CallerUserId, cancellationToken))
        {
            return Result.Forbidden("Manual activation is not permitted.");
        }

        var user = await userCommandRepository.FindAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("The user was not found.");
        }

        if (user.Status != UserStatus.Active)
        {
            user.UpdateStatus(true);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok();
    }
}
