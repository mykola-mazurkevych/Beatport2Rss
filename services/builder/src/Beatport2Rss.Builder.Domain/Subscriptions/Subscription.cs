using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Builder.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private readonly HashSet<SubscriptionTag> _tags = [];

    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SubscriptionType Type { get; private set; }
    public SubscriptionName Name { get; private set; }

    public BeatportId BeatportId { get; private set; }

    public IReadOnlySet<SubscriptionTag> Tags =>
        _tags.AsReadOnly();

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

    public bool ContainsTag(TagId tagId) =>
        _tags.Any(tag => tag.TagId == tagId);

    public void AddTag(TagId tagId) =>
        _tags.Add(SubscriptionTag.Create(Id, tagId));

    public void RemoveTag(TagId tagId) =>
        _tags.RemoveWhere(tag => tag.TagId == tagId);
}