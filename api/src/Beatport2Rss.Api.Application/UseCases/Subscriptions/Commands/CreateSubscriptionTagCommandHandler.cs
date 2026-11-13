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

public sealed record CreateSubscriptionTagCommand(
    UserId UserId,
    Slug SubscriptionSlug,
    Slug TagSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireSubscription, IRequireTag;

internal sealed class CreateSubscriptionTagCommandHandler(
    ITagQueryRepository tagQueryRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IClock clock,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateSubscriptionTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        CreateSubscriptionTagCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionCommandRepository.LoadWithTagsAsync(command.SubscriptionSlug, cancellationToken);
        var tagId = await tagQueryRepository.LoadTagIdAsync(command.UserId, command.TagSlug, cancellationToken);

        if (subscription.ContainsTag(tagId))
        {
            return Result.Conflict("Tag already exists in the subscription.");
        }

        subscription.AddTag(tagId);

        subscriptionCommandRepository.Update(subscription);

        var subscriptionTagCreated = new SubscriptionTagCreated(
            Id: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            subscription.Id.Value,
            tagId.Value);
        await integrationEventOutbox.EnqueueAsync(subscriptionTagCreated, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}