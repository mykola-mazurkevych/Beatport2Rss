using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedSubscriptionCommand(
    Guid FeedId,
    Guid SubscriptionId);

public sealed class DeleteFeedSubscriptionCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        DeleteFeedSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.FindAsync(FeedId.Create(command.FeedId), cancellationToken);
        if (feed is null)
        {
            return;
        }

        var subscriptionId = SubscriptionId.Create(command.SubscriptionId);
        if (!feed.HasSubscription(subscriptionId))
        {
            return;
        }

        feed.RemoveSubscription(subscriptionId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}