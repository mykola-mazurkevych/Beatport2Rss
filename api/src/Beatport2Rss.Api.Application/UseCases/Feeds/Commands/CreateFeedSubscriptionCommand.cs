using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedSubscriptionCommand(
    UserId UserId,
    Slug FeedSlug,
    Slug SubscriptionSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed, IRequireSubscription;

internal sealed class CreateFeedSubscriptionCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    ISubscriptionQueryRepository subscriptionQueryRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateFeedSubscriptionCommand, Result>
{
    public async ValueTask<Result> Handle(
        CreateFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadWithSubscriptionsAsync(command.UserId, command.FeedSlug, cancellationToken);
        var subscriptionId = await subscriptionQueryRepository.LoadSubscriptionIdAsync(command.SubscriptionSlug, cancellationToken);

        if (feed.HasSubscription(subscriptionId))
        {
            return Result.Conflict("Subscription already exists in the feed.");
        }

        feed.AddSubscription(subscriptionId);

        feedCommandRepository.Update(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}