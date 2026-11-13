using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;

public sealed record CreateSubscriptionTagCommand(
    Guid SubscriptionId,
    Guid TagId);

public sealed class CreateSubscriptionTagCommandHandler(
    ISubscriptionCommandRepository subscriptionCommandRepository,
    ITagCommandRepository tagCommandRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateSubscriptionTagCommandHandler> logger)
{
    public async Task HandleAsync(
        CreateSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            CreateSubscriptionTagLoggerMessages.SubscriptionNotFound(logger, command.SubscriptionId, command.TagId);
            throw new RetryableException($"Subscription {command.SubscriptionId} is not available yet.");
        }

        var tagId = TagId.Create(command.TagId);
        if (await tagCommandRepository.FindAsync(tagId, cancellationToken) is null)
        {
            CreateSubscriptionTagLoggerMessages.TagNotFound(logger, command.SubscriptionId, command.TagId);
            throw new RetryableException($"Tag {command.TagId} is not available yet.");
        }

        if (subscription.ContainsTag(tagId))
        {
            CreateSubscriptionTagLoggerMessages.TagAlreadyExists(logger, command.SubscriptionId, command.TagId);
            return;
        }

        subscription.AddTag(tagId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class CreateSubscriptionTagLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} not found while adding tag {TagId}")]
    public static partial void SubscriptionNotFound(ILogger logger, Guid subscriptionId, Guid tagId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tag {TagId} already exists in subscription {SubscriptionId}")]
    public static partial void TagAlreadyExists(ILogger logger, Guid subscriptionId, Guid tagId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tag {TagId} not found while adding it to subscription {SubscriptionId}")]
    public static partial void TagNotFound(ILogger logger, Guid subscriptionId, Guid tagId);
}