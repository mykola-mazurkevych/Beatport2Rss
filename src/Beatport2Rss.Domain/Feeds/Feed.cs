using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Subscriptions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Feeds;

public sealed class Feed :
    IAggregateRoot<FeedId>
{
    private readonly HashSet<FeedSubscription> _subscriptions = [];

    private Feed()
    {
    }

    public FeedId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public UserId UserId { get; private set; }

    public FeedName Name { get; private set; }
    public Slug Slug { get; private set; }

    public FeedStatus Status { get; private set; }

    public IReadOnlySet<FeedSubscription> Subscriptions =>
        _subscriptions.AsReadOnly();

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

    public void UpdateName(FeedName name) =>
        Name = name;

    public void UpdateSlug(Slug slug) =>
        Slug = slug;

    public void UpdateStatus(bool isActive) =>
        Status = isActive ? FeedStatus.Active : FeedStatus.Inactive;

    public void AddSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Add(FeedSubscription.Create(Id, subscriptionId));

    public void RemoveSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.RemoveWhere(subscription => subscription.SubscriptionId == subscriptionId);

    public bool HasSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Any(subscription => subscription.SubscriptionId == subscriptionId);
}