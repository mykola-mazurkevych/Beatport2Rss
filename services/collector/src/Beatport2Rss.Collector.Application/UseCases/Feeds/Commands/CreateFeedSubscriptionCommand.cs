using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedSubscriptionCommand(Guid FeedId, Guid SubscriptionId);

public sealed class CreateFeedSubscriptionCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<object>> HandleAsync(
        CreateFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        List<object> nextCommands = [];

        var feedId = FeedId.Create(command.FeedId);
        var feed =
            await feedCommandRepository.FindAsync(feedId, cancellationToken) ??
            throw new RetryableException($"Feed {command.FeedId} is not available yet.");

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (feed.HasSubscription(subscriptionId))
        {
            return nextCommands;
        }

        if (!await subscriptionCommandRepository.ExistsAsync(subscriptionId, cancellationToken))
        {
            throw new RetryableException($"Subscription {command.SubscriptionId} is not available yet.");
        }

        if (!await subscriptionCommandRepository.HasFeedsAsync(subscriptionId, cancellationToken))
        {
            nextCommands.Add(new CollectReleasesCommand(subscriptionId.Value));
        }

        feed.AddSubscription(subscriptionId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return nextCommands;
    }
}