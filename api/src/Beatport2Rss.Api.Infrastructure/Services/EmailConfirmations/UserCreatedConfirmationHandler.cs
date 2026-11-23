using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

public sealed partial class UserCreatedConfirmationHandler(
    IUserCommandRepository users,
    IEmailConfirmationCommandRepository confirmations,
    IConfirmationEmailOutbox delivery,
    IUnitOfWork unitOfWork,
    IClock clock,
    IOptions<EmailConfirmationOptions> options,
    ILogger<UserCreatedConfirmationHandler> logger)
{
    public async Task HandleAsync(UserCreated message, CancellationToken cancellationToken)
    {
        if (!options.Value.Enabled)
        {
            return;
        }

        var user = await users.FindAsync(UserId.Create(message.UserId), cancellationToken);
        if (user is null)
        {
            LogMissingUser(logger, message.UserId);
            return;
        }

        var expiresAt = user.CreatedAt + options.Value.Lifetime;
        if (expiresAt <= clock.UtcNow)
        {
            LogExpired(logger, message.UserId);
            return;
        }

        var identifier = EmailConfirmationIdentifier.Generate();
        var confirmation = EmailConfirmation.Create(
            identifier, user.CreatedAt, user.Id, user.EmailAddress, expiresAt);
        await confirmations.AddAsync(confirmation, cancellationToken);
        await delivery.EnqueueAsync(identifier);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because user {UserId} no longer exists.")]
    private static partial void LogMissingUser(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Skipped confirmation email because the link for user {UserId} has expired.")]
    private static partial void LogExpired(ILogger logger, Guid userId);
}
