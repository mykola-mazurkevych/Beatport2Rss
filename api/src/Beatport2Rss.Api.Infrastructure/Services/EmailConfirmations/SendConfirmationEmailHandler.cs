using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Wolverine.ErrorHandling;
using Wolverine.Runtime;
using Wolverine.Runtime.Handlers;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed partial class SendConfirmationEmailHandler : MessageHandler<SendConfirmationEmail>
{
    public override void ConfigureChain(HandlerChain chain) =>
        chain.OnException<PermanentConfirmationEmailException>().MoveToErrorQueue();

    protected override async Task HandleAsync(
        SendConfirmationEmail message,
        MessageContext context,
        CancellationToken cancellation)
    {
        await using var scope = context.Runtime.Services.CreateAsyncScope();
        await DeliverAsync(message, scope.ServiceProvider, cancellation);
    }

    internal static async Task DeliverAsync(
        SendConfirmationEmail message,
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var logger = services.GetRequiredService<ILogger<SendConfirmationEmailHandler>>();
        var options = services.GetRequiredService<IOptions<EmailConfirmationOptions>>().Value;
        if (!options.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        if (EmailConfirmationIdentifier.Parse(message.Identifier) is not { } id)
        {
            LogInvalidMessage(logger);
            return;
        }

        var repository = services.GetRequiredService<IEmailConfirmationQueryRepository>();
        var clock = services.GetRequiredService<IClock>();
        var confirmation = await repository.FindAsync(id, cancellationToken);
        if (confirmation is null || confirmation.ConfirmedAt is not null || confirmation.ExpiresAt <= clock.UtcNow)
        {
            LogUnsendableConfirmation(logger);
            return;
        }

        var delivery = services.GetRequiredService<ResendConfirmationEmail>();
        await delivery.SendAsync(confirmation, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because delivery is disabled.")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Skipped confirmation email because the delivery message is invalid.")]
    private static partial void LogInvalidMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because the confirmation is missing, expired, or already completed.")]
    private static partial void LogUnsendableConfirmation(ILogger logger);
}
