using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedSubscriptionCommand(
    UserId UserId,
    Slug FeedSlug,
    BeatportSubscriptionType BeatportType,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed, IRequireSubscription;

internal sealed class CreateFeedSubscriptionCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateFeedSubscriptionCommand, Result>
{
    public async ValueTask<Result> Handle(
        CreateFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadWithSubscriptionsAsync(command.UserId, command.FeedSlug, cancellationToken);
        var subscription = await subscriptionCommandRepository
            .LoadAsync(
                s =>
                    s.BeatportType == command.BeatportType &&
                    s.BeatportId == command.BeatportId &&
                    s.BeatportSlug == command.BeatportSlug,
                cancellationToken);

        if (feed.HasSubscription(subscription.Id))
        {
            return Result.Conflict("Subscription already exists in the feed.");
        }

        feed.AddSubscription(subscription.Id);

        feedCommandRepository.Update(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}