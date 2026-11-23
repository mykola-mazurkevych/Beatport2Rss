using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

public sealed record SendConfirmationEmailCommand(Guid EmailConfirmationId);

public sealed class SendConfirmationEmailCommandHandler(
    IEmailConfirmationQueryRepository emailConfirmationQueryRepository,
    Func<IConfirmationEmailSender> delivery, // TOOD: WTF??
    IClock clock,
    IOptions<EmailConfirmationOptions> emailConfirmationOptions,
    ILogger<SendConfirmationEmailCommandHandler> logger)
{
    private readonly EmailConfirmationOptions _emailConfirmationOptions = emailConfirmationOptions.Value;

    public async Task HandleAsync(SendConfirmationEmailCommand command, CancellationToken cancellationToken)
    {
        if (!_emailConfirmationOptions.Enabled)
        {
            SendConfirmationEmailCommandHandlerLoggerMessages.EmailConfirmationDisabled(logger);
            return;
        }

        var emailConfirmationId = EmailConfirmationId.Create(command.EmailConfirmationId);
        var emailConfirmation = await emailConfirmationQueryRepository.FindAsync(emailConfirmationId, cancellationToken);
        if (emailConfirmation is null)
        {
            SendConfirmationEmailCommandHandlerLoggerMessages.EmailConfirmationNotFound(logger, emailConfirmationId.Value);
            return;
        }

        if (emailConfirmation.ConfirmedAt is not null)
        {
            SendConfirmationEmailCommandHandlerLoggerMessages.EmailConfirmationAlreadyConfirmed(logger, emailConfirmationId.Value);
            return;
        }

        if (emailConfirmation.ExpiresAt <= clock.UtcNow)
        {
            SendConfirmationEmailCommandHandlerLoggerMessages.EmailConfirmationExpired(logger, emailConfirmationId.Value);
            return;
        }

        await delivery().SendAsync(emailConfirmation, cancellationToken);
    }
}

internal static partial class SendConfirmationEmailCommandHandlerLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Email confirmation disabled.")]
    public static partial void EmailConfirmationDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Email confirmation {EmailConfirmationId} not found.")]
    public static partial void EmailConfirmationNotFound(ILogger logger, Guid emailConfirmationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Email confirmation {EmailConfirmationId} is already confirmed.")]
    public static partial void EmailConfirmationAlreadyConfirmed(ILogger logger, Guid emailConfirmationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Email confirmation {EmailConfirmationId} is expired.")]
    public static partial void EmailConfirmationExpired(ILogger logger, Guid emailConfirmationId);
}