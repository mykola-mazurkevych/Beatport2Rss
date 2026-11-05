using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionCommand(
    Guid Id,
    DateTimeOffset CreatedAt,
    string Name,
    SubscriptionType Type,
    int BeatportId,
    string BeatportSlug);

public static class CreateSubscriptionCommandHandler
{
    public static async Task HandleAsync(
        CreateSubscriptionCommand command,
        ISubscriptionCommandRepository subscriptionCommandRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSubscriptionCommand> logger,
        CancellationToken cancellationToken)
    {
        CreateSubscriptionCommandLoggerMessages.Creating(logger, command.Id, command.Name, command.Type);

        var subscriptionId = SubscriptionId.Create(command.Id);
        if (await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            CreateSubscriptionCommandLoggerMessages.AlreadyExists(logger, command.Id);
            return;
        }

        var subscription = Subscription.Create(
            subscriptionId,
            command.CreatedAt,
            command.Type,
            BeatportId.Create(command.BeatportId),
            BeatportSlug.Create(command.BeatportSlug),
            subscribersCount: 0,
            refreshedAt: null);

        await subscriptionCommandRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        CreateSubscriptionCommandLoggerMessages.Created(logger, command.Id);
    }
}

internal static partial class CreateSubscriptionCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating subscription {SubscriptionId} (Name: {SubscriptionName}, Type: {SubscriptionType})")]
    public static partial void Creating(ILogger logger, Guid subscriptionId, string subscriptionName, SubscriptionType subscriptionType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} already exists")]
    public static partial void AlreadyExists(ILogger logger, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription {SubscriptionId} created")]
    public static partial void Created(ILogger logger, Guid subscriptionId);
}