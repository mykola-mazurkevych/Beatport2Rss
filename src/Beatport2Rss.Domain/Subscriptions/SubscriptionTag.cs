using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Subscriptions;

public sealed record SubscriptionTag :
    IValueObject
{
    private SubscriptionTag()
    {
    }

    public SubscriptionId SubscriptionId { get; private set; }
    public TagId TagId { get; private set; }

    public static SubscriptionTag Create(
        SubscriptionId subscriptionId,
        TagId tagId) =>
        new()
        {
            SubscriptionId = subscriptionId,
            TagId = tagId,
        };
}