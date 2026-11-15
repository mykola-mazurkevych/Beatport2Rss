using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedSubscriptionCommand(
    Guid FeedId,
    Guid SubscriptionId);

public sealed class CreateFeedSubscriptionCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateFeedSubscriptionCommandHandler> logger)
{
    public async Task HandleAsync(
        CreateFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var feedId = FeedId.Create(command.FeedId);
        var feed = await feedCommandRepository.FindAsync(feedId, cancellationToken);
        if (feed is null)
        {
            CreateFeedSubscriptionCommandLoggerMessages.FeedNotFound(logger, command.FeedId, command.SubscriptionId);
            throw new RetryableException($"Feed {command.FeedId} is not available yet.");
        }

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (feed.HasSubscription(subscriptionId))
        {
            return;
        }

        if (!await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            CreateFeedSubscriptionCommandLoggerMessages.SubscriptionNotFound(logger, command.FeedId, command.SubscriptionId);
            throw new RetryableException($"Subscription {command.SubscriptionId} is not available yet.");
        }

        feed.AddSubscription(subscriptionId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class CreateFeedSubscriptionCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Feed {FeedId} not found while adding subscription {SubscriptionId}")]
    public static partial void FeedNotFound(ILogger logger, Guid feedId, Guid subscriptionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} not found while adding it to feed {FeedId}")]
    public static partial void SubscriptionNotFound(ILogger logger, Guid feedId, Guid subscriptionId);
}