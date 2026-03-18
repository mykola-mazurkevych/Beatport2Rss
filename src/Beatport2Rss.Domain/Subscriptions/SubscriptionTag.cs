using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Subscriptions;

public sealed record SubscriptionTag :
    IValueObject
{
    private SubscriptionTag()
    {
    }

    public TagId TagId { get; private set; }

    public static SubscriptionTag Create(TagId tagId) =>
        new() { TagId = tagId };
}