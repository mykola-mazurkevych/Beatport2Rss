using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Feeds;

public sealed class Feed :
    IEntity<FeedId>
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

    public IReadOnlySet<SubscriptionId> SubscriptionIds =>
        _subscriptionIds.AsReadOnly();

    public bool IsActive =>
        Status == FeedStatus.Active;

    public static Feed Create(
        FeedId id,
        DateTimeOffset createdAt,
        UserId userId,
        FeedName name,
        Slug slug,
        bool isActive) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
            Name = name,
            Slug = slug,
            Status = isActive? FeedStatus.Active : FeedStatus.Inactive,
        };

    internal void UpdateName(FeedName name) =>
        Name = name;

    internal void UpdateSlug(Slug slug) =>
        Slug = slug;

    internal void UpdateStatus(bool isActive) =>
        Status = isActive ? FeedStatus.Active : FeedStatus.Inactive;

    internal void AddSubscription(SubscriptionId subscriptionId) =>
        _subscriptionIds.Add(subscriptionId);

    internal void RemoveSubscription(SubscriptionId subscriptionId) =>
        _subscriptionIds.Remove(subscriptionId);

    internal bool HasSubscription(SubscriptionId subscriptionId) =>
        _subscriptionIds.Contains(subscriptionId);
}