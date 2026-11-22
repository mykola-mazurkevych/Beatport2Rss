using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
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

        if (message.Identifier is not { } identifier || EmailConfirmationIdentifier.Hash(identifier) is not { } hash)
        {
            LogInvalidMessage(logger);
            return;
        }

        var repository = services.GetRequiredService<IEmailVerificationQueryRepository>();
        var clock = services.GetRequiredService<IClock>();
        var verification = await repository.FindAsync(hash, cancellationToken);
        if (verification is null || verification.ConfirmedAt is not null || verification.ExpiresAt <= clock.UtcNow)
        {
            LogUnsendableVerification(logger);
            return;
        }

        var delivery = services.GetRequiredService<ResendConfirmationEmail>();
        await delivery.SendAsync(verification, identifier, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because delivery is disabled.")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Skipped confirmation email because the delivery message is invalid.")]
    private static partial void LogInvalidMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because the verification is missing, expired, or already confirmed.")]
    private static partial void LogUnsendableVerification(ILogger logger);
}
