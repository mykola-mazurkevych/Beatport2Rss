using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailVerifications;
using Beatport2Rss.Api.Domain.EmailVerifications;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailVerifications;

public sealed record ConfirmEmailCommand(EmailVerificationSecretHash? Hash, bool? Confirmed) : ICommand<Result>;

internal sealed class ConfirmEmailCommandHandler(
    IEmailVerificationCommandRepository repository,
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

        return options.Value.Enabled && command.Hash is { } hash &&
            await repository.ConfirmAsync(hash, clock, cancellationToken)
            ? Result.Ok()
            : EmailConfirmationErrors.InvalidLinkResult();
    }
}
