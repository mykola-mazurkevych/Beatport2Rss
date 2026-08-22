using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.V1;
using Beatport2Rss.Common.SharedKernel.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedSubscriptionCommand(
    UserId UserId,
    Slug FeedSlug,
    Slug SubscriptionSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed, IRequireSubscription;

internal sealed class DeleteFeedSubscriptionCommandHandler(
    IClock clock,
    IFeedCommandRepository feedCommandRepository,
    ISubscriptionQueryRepository subscriptionQueryRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedSubscriptionCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadWithSubscriptionsAsync(command.UserId, command.FeedSlug, cancellationToken);
        var subscriptionId = await subscriptionQueryRepository.LoadSubscriptionIdAsync(command.SubscriptionSlug, cancellationToken);

        if (!feed.HasSubscription(subscriptionId))
        {
            return Result.NotFound("Subscription does not exist in the feed.");
        }

        feed.RemoveSubscription(subscriptionId);
        feedCommandRepository.Update(feed);

        var feedSubscriptionDeleted = new FeedSubscriptionDeletedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            FeedId: feed.Id.Value,
            SubscriptionId: subscriptionId.Value);
        integrationEventOutbox.Enqueue(feedSubscriptionDeleted);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}