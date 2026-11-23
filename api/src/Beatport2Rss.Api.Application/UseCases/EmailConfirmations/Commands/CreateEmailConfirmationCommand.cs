using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;

public sealed record CreateEmailConfirmationCommand(Guid UserId);

public sealed class CreateEmailConfirmationCommandHandler(
    IUserCommandRepository users,
    IEmailConfirmationCommandRepository emailConfirmationCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork,
    IClock clock,
    IOptions<EmailConfirmationOptions> emailConfirmationOptions,
    ILogger<CreateEmailConfirmationCommandHandler> logger)
{
    private readonly EmailConfirmationOptions _emailConfirmationOptions = emailConfirmationOptions.Value;

    public async Task HandleAsync(CreateEmailConfirmationCommand command, CancellationToken cancellationToken)
    {
        if (!_emailConfirmationOptions.Enabled)
        {
            CreateEmailConfirmationCommandHandlerLoggerMessages.EmailConfirmationDisabled(logger);
            return;
        }

        var user = await users.FindAsync(UserId.Create(command.UserId), cancellationToken);
        if (user is null)
        {
            CreateEmailConfirmationCommandHandlerLoggerMessages.UserNotFound(logger, command.UserId);
            return;
        }

        var emailConfirmationId = EmailConfirmationId.Create(Guid.NewGuid());
        var emailConfirmation = EmailConfirmation.Create(
            emailConfirmationId,
            createdAt: clock.UtcNow,
            user.Id,
            user.EmailAddress,
            expiresAt: clock.UtcNow + _emailConfirmationOptions.Lifetime);
        await emailConfirmationCommandRepository.AddAsync(emailConfirmation, cancellationToken);

        var emailConfirmationCreated = new EmailConfirmationCreated(
            Id: Guid.NewGuid(),
            OccurredAt: clock.UtcNow,
            emailConfirmationId.Value);
        await integrationEventOutbox.EnqueueAsync(emailConfirmationCreated, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class CreateEmailConfirmationCommandHandlerLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Email confirmation disabled.")]
    public static partial void EmailConfirmationDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} not found.")]
    public static partial void UserNotFound(ILogger logger, Guid userId);
}