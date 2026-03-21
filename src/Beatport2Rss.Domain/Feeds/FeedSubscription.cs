using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Feeds;

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