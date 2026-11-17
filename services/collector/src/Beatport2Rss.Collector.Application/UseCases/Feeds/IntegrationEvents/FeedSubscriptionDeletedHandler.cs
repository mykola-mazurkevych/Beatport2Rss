using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedSubscriptionDeletedHandler
{
    public static DeleteFeedSubscriptionCommand Handle(FeedSubscriptionDeleted feedSubscriptionDeleted) =>
        new(feedSubscriptionDeleted.FeedId,
            feedSubscriptionDeleted.SubscriptionId);
}