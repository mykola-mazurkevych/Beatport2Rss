using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Releases;

public sealed record ReleaseSubscription :
    IValueObject
{
    private ReleaseSubscription()
    {
    }

    public ReleaseId ReleaseId { get; private set; }
    public SubscriptionId SubscriptionId { get; private set; }

    public ReleaseSubscriptionType Type { get; private set; }

    public static ReleaseSubscription Create(
        ReleaseId releaseId,
        SubscriptionId subscriptionId,
        ReleaseSubscriptionType type) =>
        new()
        {
            ReleaseId = releaseId,
            SubscriptionId = subscriptionId,
            Type = type,
        };
}
