using Beatport2Rss.Builder.Domain.Common.ValueObjects;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SubscriptionType Type { get; private set; }
    public SubscriptionName Name { get; private set; }

    public BeatportId BeatportId { get; private set; }

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionType type,
        SubscriptionName name,
        BeatportId beatportId) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            Name = name,
            BeatportId = beatportId,
        };
}