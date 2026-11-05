using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;

public sealed record IncrementSubscribersCountCommand(Guid SubscriptionId);

public static class IncrementSubscribersCountCommandHandler
{
    public static async Task HandleAsync(
        IncrementSubscribersCountCommand command,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        ILogger<IncrementSubscribersCountCommand> logger,
        CancellationToken cancellationToken = default)
    {
        IncrementSubscribersCountCommandLoggerMessages.Incrementing(logger, command.SubscriptionId);

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            IncrementSubscribersCountCommandLoggerMessages.NotFound(logger, command.SubscriptionId);
            return;
        }

        subscription.IncrementSubscribersCount();
        subscriptionCommandRepository.Update(subscription);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        IncrementSubscribersCountCommandLoggerMessages.Incremented(logger, command.SubscriptionId, subscription.SubscribersCount);
    }
}

internal static partial class IncrementSubscribersCountCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Incrementing subscribers count for subscription {SubscriptionId}")]
    public static partial void Incrementing(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} not found")]
    public static partial void NotFound(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Incremented subscribers count for subscription {SubscriptionId} to {SubscribersCount}")]
    public static partial void Incremented(ILogger logger, Guid subscriptionId, int subscribersCount);
}