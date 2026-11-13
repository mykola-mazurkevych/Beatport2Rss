using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;

public sealed record DeleteSubscriptionTagCommand(
    UserId UserId,
    Slug SubscriptionSlug,
    Slug TagSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireSubscription, IRequireTag;

internal sealed class DeleteSubscriptionTagCommandHandler(
    ITagQueryRepository tagQueryRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IClock clock,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSubscriptionTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(command.SubscriptionSlug, cancellationToken);
        var tagId = await tagQueryRepository.LoadTagIdAsync(command.UserId, command.TagSlug, cancellationToken);

        if (!subscription.ContainsTag(tagId))
        {
            return Result.NotFound("The subscription does not contain the tag.");
        }

        subscription.RemoveTag(tagId);

        subscriptionCommandRepository.Update(subscription);

        var subscriptionTagDeleted = new SubscriptionTagDeleted(
            Id: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            subscription.Id.Value,
            tagId.Value);
        await integrationEventOutbox.EnqueueAsync(subscriptionTagDeleted, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}