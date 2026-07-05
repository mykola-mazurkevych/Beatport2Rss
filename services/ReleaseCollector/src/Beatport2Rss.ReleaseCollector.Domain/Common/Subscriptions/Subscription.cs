using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.ReleaseCollector.Domain.Common.Subscriptions;

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
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Name = name;
        BeatportId = beatportId;
        BeatportSlug = beatportSlug;
        SubscribersCount = subscribersCount;
        RefreshedAt = refreshedAt;
    }

    protected Subscription()
    {
    }

    public TSubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public TSubscriptionName Name { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int SubscribersCount { get; private set; }
    public DateTimeOffset? RefreshedAt { get; private set; }
}