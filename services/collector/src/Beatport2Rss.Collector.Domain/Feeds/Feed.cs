using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Collector.Domain.Feeds;

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

    public IReadOnlySet<FeedSubscription> Subscriptions => _subscriptions.AsReadOnly();

    public static Feed Create(
        FeedId id,
        DateTimeOffset createdAt,
        UserId userId) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
        };

    public bool HasSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Any(subscription => subscription.SubscriptionId == subscriptionId);

    public void AddSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Add(FeedSubscription.Create(Id, subscriptionId));

    public void RemoveSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.RemoveWhere(subscription => subscription.SubscriptionId == subscriptionId);
}