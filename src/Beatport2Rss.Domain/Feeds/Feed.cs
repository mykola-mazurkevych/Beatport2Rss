using Beatport2Rss.SharedKernel;
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

    public FeedName Name { get; private set; }
    public string Slug { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public UserId UserId { get; private set; }

    public IReadOnlySet<SubscriptionId> SubscriptionIds => _subscriptionIds.AsReadOnly();

    public static Feed Create(
        FeedId id,
        FeedName name,
        string slug,
        bool isActive,
        DateTimeOffset createdDate,
        UserId userId) =>
        new()
        {
            Id = id,
            Name = name,
            Slug = slug,
            IsActive = isActive,
            CreatedDate = createdDate,
            UserId = userId,
        };

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Add(subscriptionId);
    public void RemoveSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Remove(subscriptionId);
    public bool HasSubscription(SubscriptionId subscriptionId) => _subscriptionIds.Contains(subscriptionId);
}