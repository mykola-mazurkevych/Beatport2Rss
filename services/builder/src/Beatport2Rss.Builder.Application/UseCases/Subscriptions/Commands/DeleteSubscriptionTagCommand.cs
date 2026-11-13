using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagCommand(
    Guid SubscriptionId,
    Guid TagId);

public sealed class DeleteSubscriptionTagCommandHandler(
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteSubscriptionTagCommandHandler> logger)
{
    public async Task HandleAsync(
        DeleteSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        var subscription = await subscriptionCommandRepository.FindAsync(subscriptionId, cancellationToken);
        if (subscription is null)
        {
            DeleteSubscriptionTagLoggerMessages.SubscriptionNotFound(logger, command.SubscriptionId, command.TagId);
            return;
        }

        var tagId = TagId.Create(command.TagId);
        if (!subscription.ContainsTag(tagId))
        {
            DeleteSubscriptionTagLoggerMessages.TagNotFound(logger, command.SubscriptionId, command.TagId);
            return;
        }

        subscription.RemoveTag(tagId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class DeleteSubscriptionTagLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Subscription {SubscriptionId} not found while removing tag {TagId}")]
    public static partial void SubscriptionNotFound(ILogger logger, Guid subscriptionId, Guid tagId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tag {TagId} not found in subscription {SubscriptionId}")]
    public static partial void TagNotFound(ILogger logger, Guid subscriptionId, Guid tagId);
}