using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

public sealed record ConfirmEmailCommand(EmailConfirmationId? Id, bool? Confirmed) : ICommand<Result>;

internal sealed class ConfirmEmailCommandHandler(
    IEmailConfirmationCommandRepository repository,
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

        return options.Value.Enabled && command.Id is { } id &&
            await repository.ConfirmAsync(id, clock, cancellationToken)
            ? Result.Ok()
            : EmailConfirmationErrors.InvalidLinkResult();
    }
}
