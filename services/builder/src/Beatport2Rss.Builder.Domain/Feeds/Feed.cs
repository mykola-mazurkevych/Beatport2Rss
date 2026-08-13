using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Builder.Domain.Feeds;

public sealed class Feed :
    IAggregateRoot<FeedId>
{
    private readonly HashSet<FeedSubscription> _subscriptions = [];

    private Feed()
    {
    }

    public FeedId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public FeedName Name { get; private set; }
    public Slug Slug { get; private set; }
    public AuthorName AuthorName { get; private set; }

    public IReadOnlySet<FeedSubscription> Subscriptions =>
        _subscriptions.AsReadOnly();

    public static Feed Create(
        FeedId id,
        DateTimeOffset createdAt,
        FeedName name,
        Slug slug,
        AuthorName authorName) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Name = name,
            Slug = slug,
            AuthorName = authorName,
        };

    public void UpdateName(FeedName name) =>
        Name = name;

    public void UpdateSlug(Slug slug) =>
        Slug = slug;

    public void AddSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Add(FeedSubscription.Create(Id, subscriptionId));

    public void RemoveSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.RemoveWhere(subscription => subscription.SubscriptionId == subscriptionId);

    public bool HasSubscription(SubscriptionId subscriptionId) =>
        _subscriptions.Any(subscription => subscription.SubscriptionId == subscriptionId);
}