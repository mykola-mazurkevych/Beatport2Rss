using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Domain.Feeds;

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