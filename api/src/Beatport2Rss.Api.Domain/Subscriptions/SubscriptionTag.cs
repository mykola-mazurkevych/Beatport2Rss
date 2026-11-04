using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Api.Domain.Subscriptions;

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