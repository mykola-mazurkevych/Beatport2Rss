using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedSubscriptionDeletedHandler
{
    public static DecrementSubscribersCountCommand Handle(FeedSubscriptionDeleted feedSubscriptionDeleted) =>
        feedSubscriptionDeleted.ToCommand();

    extension(FeedSubscriptionDeleted feedSubscriptionDeleted)
    {
        private DecrementSubscribersCountCommand ToCommand() =>
            new(feedSubscriptionDeleted.SubscriptionId);
    }
}