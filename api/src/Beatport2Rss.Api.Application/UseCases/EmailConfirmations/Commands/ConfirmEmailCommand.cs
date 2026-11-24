using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

public sealed record ConfirmEmailCommand(EmailConfirmationId? Id, bool? Confirmed) : ICommand<Result>;

internal sealed class ConfirmEmailCommandHandler(
    IEmailConfirmationCommandRepository repository,
    IUserCommandRepository users,
    ITransactionFactory transactionFactory,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork,
    IClock clock,
    IOptions<EmailConfirmationOptions> options) :
    ICommandHandler<ConfirmEmailCommand, Result>
{
    public async ValueTask<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        if (command.Confirmed is not true)
        {
            return EmailConfirmationErrors.ConfirmationRequiredResult();
        }

        if (!options.Value.Enabled || command.Id is not { } id)
        {
            return EmailConfirmationErrors.InvalidLinkResult();
        }

        await using var transaction = await transactionFactory.BeginAsync(cancellationToken);
        var userId = await repository.FindUserIdAsync(id, cancellationToken);
        if (userId is null)
        {
            return EmailConfirmationErrors.InvalidLinkResult();
        }

        var user = await users.FindForUpdateAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return EmailConfirmationErrors.InvalidLinkResult();
        }

        var confirmation = await repository.FindAsync(id, cancellationToken);
        if (confirmation is null)
        {
            return EmailConfirmationErrors.InvalidLinkResult();
        }

        var alreadyConfirmed = confirmation.ConfirmedAt is not null;
        var now = clock.UtcNow;
        if (!confirmation.TryConfirm(user, now))
        {
            return EmailConfirmationErrors.InvalidLinkResult();
        }

        if (!alreadyConfirmed)
        {
            await integrationEventOutbox.EnqueueAsync(
                new EmailConfirmed(Guid.NewGuid(), now, user.Id.Value, confirmation.EmailAddress.Value),
                cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok();
    }
}
