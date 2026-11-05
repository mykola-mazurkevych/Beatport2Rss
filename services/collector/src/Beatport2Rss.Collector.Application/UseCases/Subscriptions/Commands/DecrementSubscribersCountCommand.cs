using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;

public sealed record DecrementSubscribersCountCommand(Guid SubscriptionId);

public static class DecrementSubscribersCountCommandHandler
{
    public static async Task HandleAsync(
        DecrementSubscribersCountCommand command,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        ILogger<DecrementSubscribersCountCommand> logger,
        CancellationToken cancellationToken = default)
    {
        DecrementSubscribersCountCommandLoggerMessages.Decrementing(logger, command.SubscriptionId);

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            DecrementSubscribersCountCommandLoggerMessages.NotFound(logger, command.SubscriptionId);
            return;
        }

        if (!subscription.DecrementSubscribersCount())
        {
            DecrementSubscribersCountCommandLoggerMessages.AlreadyZero(logger, command.SubscriptionId);
            return;
        }

        subscriptionCommandRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DecrementSubscribersCountCommandLoggerMessages.Decremented(
            logger,
            command.SubscriptionId,
            subscription.SubscribersCount);
    }
}

internal static partial class DecrementSubscribersCountCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Decrementing subscribers count for subscription {SubscriptionId}")]
    public static partial void Decrementing(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} not found")]
    public static partial void NotFound(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscribers count for subscription {SubscriptionId} is already zero")]
    public static partial void AlreadyZero(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Decremented subscribers count for subscription {SubscriptionId} to {SubscribersCount}")]
    public static partial void Decremented(ILogger logger, Guid subscriptionId, int subscribersCount);
}
