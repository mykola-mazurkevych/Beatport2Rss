using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Builder.Domain.Feeds;

public sealed record FeedSubscription :
    IValueObject
{
    private FeedSubscription()
    {
    }

    public FeedId FeedId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }

    public static FeedSubscription Create(
        FeedId feedId,
        SubscriptionId subscriptionId) =>
        new()
        {
            FeedId = feedId,
            SubscriptionId = subscriptionId,
        };
}