using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    SubscriptionId Id,
    DateTimeOffset CreatedAt,
    SubscriptionName Name,
    SubscriptionType Type,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug);

public static class CreateSubscriptionCommandHandler
{
    public static async Task HandleAsync(
        CreateSubscriptionCommand command,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSubscriptionCommand> logger,
        CancellationToken cancellationToken)
    {
        LoggerMessages.Creating(logger, command.Id, command.Name, command.Type);

        if (await subscriptionCommandRepository.ExistsAsync(command.Id, cancellationToken))
        {
            LoggerMessages.AlreadyExists(logger, command.Id);
            return;
        }

        var subscription = Subscription.Create(
            command.Id,
            command.CreatedAt,
            command.Type,
            command.BeatportId,
            command.BeatportSlug,
            subscribersCount: 0,
            refreshedAt: null);

        await subscriptionCommandRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        LoggerMessages.Created(logger, command.Id);
    }
}

internal static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating subscription {SubscriptionId} (Name: {SubscriptionName}, Type: {SubscriptionType})")]
    public static partial void Creating(ILogger logger, SubscriptionId subscriptionId, SubscriptionName subscriptionName, SubscriptionType subscriptionType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} already exists")]
    public static partial void AlreadyExists(ILogger logger, SubscriptionId subscriptionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription {SubscriptionId} created")]
    public static partial void Created(ILogger logger, SubscriptionId subscriptionId);
}