using Beatport2Rss.Builder.Domain.Common.ValueObjects;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Common.Subscriptions;

public abstract class Subscription<TSubscriptionId, TSubscriptionName> :
    IAggregateRoot<TSubscriptionId>
    where TSubscriptionId : struct, IId<TSubscriptionId>
    where TSubscriptionName : struct
{
    protected Subscription(
        TSubscriptionId id,
        DateTimeOffset createdAt,
        TSubscriptionName name,
        BeatportId beatportId,
        Uri beatportUri)
    {
        Id = id;
        CreatedAt = createdAt;
        Name = name;
        BeatportId = beatportId;
        BeatportUri = beatportUri;
    }

    protected Subscription()
    {
    }

    public TSubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public TSubscriptionName Name { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public Uri BeatportUri { get; private set; } = null!;
}