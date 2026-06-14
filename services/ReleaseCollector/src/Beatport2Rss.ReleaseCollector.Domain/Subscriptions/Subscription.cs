// ReSharper disable PropertyCanBeMadeInitOnly.Local

using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.ReleaseCollector.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SubscriptionName Name { get; private set; }

    public BeatportSubscriptionType BeatportType { get; private set; }
    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int SubscribersCount { get; private set; }
    public DateTimeOffset? RefreshedAt { get; private set; }

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionName name,
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Name = name,
            BeatportType = beatportType,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            SubscribersCount = subscribersCount,
            RefreshedAt = refreshedAt,
        };
}