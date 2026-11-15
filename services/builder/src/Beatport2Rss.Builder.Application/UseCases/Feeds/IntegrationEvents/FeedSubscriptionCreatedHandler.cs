using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedSubscriptionCreatedHandler
{
    public static CreateFeedSubscriptionCommand Handle(FeedSubscriptionCreated feedSubscriptionCreated) =>
        new(feedSubscriptionCreated.FeedId,
            feedSubscriptionCreated.SubscriptionId);
}