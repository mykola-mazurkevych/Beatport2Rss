using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Domain.Feeds;

public sealed class Feed : IEntity<FeedId>
{
    private readonly HashSet<SubscriptionId> _subscriptionIds = [];

    private Feed()
    {
    }

    public FeedId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public UserId UserId { get; private set; }

    public FeedName Name { get; private set; }
    public Slug Slug { get; private set; }

    public FeedStatus Status { get; private set; }

    public IReadOnlySet<SubscriptionId> SubscriptionIds => _subscriptionIds.AsReadOnly();

    public static Feed Create(
        FeedId id,
        DateTimeOffset createdAt,
        UserId userId,
        FeedName name,
        Slug slug,
        FeedStatus status) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
            Name = name,
            Slug = slug,
            Status = status,
        };
    
    internal void UpdateName(FeedName name) =>
        Name = name;

    internal void UpdateStatus(FeedStatus status) =>
        Status = status;

    internal void AddSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Add(subscriptionId);
    internal void RemoveSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Remove(subscriptionId);
    internal bool HasSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Contains(subscriptionId);
}