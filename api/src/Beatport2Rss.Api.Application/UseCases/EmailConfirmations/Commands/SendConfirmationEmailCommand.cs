using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

public sealed record SendConfirmationEmailCommand(string Identifier)
{
    public override string ToString() => nameof(SendConfirmationEmailCommand);
}

public sealed partial class SendConfirmationEmailCommandHandler(
    IEmailConfirmationQueryRepository repository,
    Func<IConfirmationEmailSender> delivery,
    IClock clock,
    IOptions<EmailConfirmationOptions> options,
    ILogger<SendConfirmationEmailCommandHandler> logger)
{
    public async Task HandleAsync(SendConfirmationEmailCommand command, CancellationToken cancellationToken)
    {
        if (!options.Value.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        if (EmailConfirmationIdentifier.Parse(command.Identifier) is not { } id)
        {
            LogInvalidMessage(logger);
            return;
        }

        var confirmation = await repository.FindAsync(id, cancellationToken);
        if (confirmation is null || confirmation.ConfirmedAt is not null || confirmation.ExpiresAt <= clock.UtcNow)
        {
            LogUnsendableConfirmation(logger);
            return;
        }

        await delivery().SendAsync(confirmation, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because delivery is disabled.")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Skipped confirmation email because the delivery message is invalid.")]
    private static partial void LogInvalidMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because the confirmation is missing, expired, or already completed.")]
    private static partial void LogUnsendableConfirmation(ILogger logger);
}
