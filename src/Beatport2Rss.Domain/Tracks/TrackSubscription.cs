using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Tracks;

public sealed record TrackSubscription :
    IValueObject
{
    private TrackSubscription()
    {
    }

    public TrackId TrackId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }
    public TrackSubscriptionType Type { get; private set; }

    public static TrackSubscription Create(
        TrackId trackId,
        SubscriptionId subscriptionId,
        TrackSubscriptionType type) =>
        new()
        {
            TrackId = trackId,
            SubscriptionId = subscriptionId,
            Type = type,
        };
}