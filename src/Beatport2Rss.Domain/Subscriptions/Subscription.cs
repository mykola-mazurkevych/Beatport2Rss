// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private readonly HashSet<SubscriptionTag> _tags = [];

    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportSubscriptionType BeatportType { get; private set; }
    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public string Name { get; private set; } = null!;

    public Uri ImageUri { get; private set; } = null!;

    public DateTimeOffset? RefreshedAt { get; private set; }

    public IReadOnlySet<SubscriptionTag> Tags =>
        _tags.AsReadOnly();

    public static Subscription Create(
        DateTimeOffset createdAt,
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        string name,
        Uri imageUri,
        DateTimeOffset? refreshedAt) =>
        new()
        {
            CreatedAt = createdAt,
            BeatportType = beatportType,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Name = name,
            ImageUri = imageUri,
            RefreshedAt = refreshedAt,
        };

    public void MarkAsRefreshed(DateTimeOffset refreshedAt) =>
        RefreshedAt = refreshedAt;

    public bool ContainsTag(TagId tagId) =>
        _tags.Any(t => t.TagId == tagId);

    public void AddTag(TagId tagId) =>
        _tags.Add(SubscriptionTag.Create(tagId));

    public void RemoveTag(TagId tagId) =>
        _tags.RemoveWhere(t => t.TagId == tagId);

    public void RemoveTags() =>
        _tags.Clear();
}