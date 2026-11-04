using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    Guid SubscriptionId,
    DateTimeOffset CreatedAt,
    string Name,
    SubscriptionType Type,
    int BeatportId);

public static class CreateSubscriptionCommandHandler
{
    public static async Task Handle(
        CreateSubscriptionCommand command,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSubscriptionCommand> logger,
        CancellationToken cancellationToken)
    {
        LoggerMessages.Creating(logger, command.SubscriptionId, command.Name, command.Type);

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            LoggerMessages.AlreadyExists(logger, command.SubscriptionId);
            return;
        }

        var subscription = Subscription.Create(
            subscriptionId,
            command.CreatedAt,
            command.Type,
            SubscriptionName.Create(command.Name),
            BeatportId.Create(command.BeatportId));

        await subscriptionCommandRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        LoggerMessages.Created(logger, command.SubscriptionId);
    }
}

internal static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating subscription {SubscriptionId} (Name: {SubscriptionName}, Type: {SubscriptionType})")]
    public static partial void Creating(ILogger logger, Guid subscriptionId, string subscriptionName, SubscriptionType subscriptionType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} already exists")]
    public static partial void AlreadyExists(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription {SubscriptionId} created")]
    public static partial void Created(ILogger logger, Guid subscriptionId);
}