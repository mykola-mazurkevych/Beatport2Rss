using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Collector.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SubscriptionType Type { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int SubscribersCount { get; private set; }
    public DateTimeOffset? RefreshedAt { get; private set; }

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionType type,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            SubscribersCount = subscribersCount,
            RefreshedAt = refreshedAt,
        };

    public void IncrementSubscribersCount() => SubscribersCount++;

    public bool DecrementSubscribersCount()
    {
        if (SubscribersCount == 0)
        {
            return false;
        }

        SubscribersCount--;
        return true;
    }
}