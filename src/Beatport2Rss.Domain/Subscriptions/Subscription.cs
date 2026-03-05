// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Subscriptions;

public sealed class Subscription :
    IAggregateRoot<SubscriptionId>
{
    private readonly HashSet<FeedId> _feedIds = [];
    private readonly HashSet<TagId> _tagIds = [];

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

    public IReadOnlySet<FeedId> FeedIds =>
        _feedIds.AsReadOnly();

    public IReadOnlySet<TagId> TagIds =>
        _tagIds.AsReadOnly();

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

    public void AddTag(TagId tagId) =>
        _tagIds.Add(tagId);

    public void RemoveTag(TagId tagId) =>
        _tagIds.Remove(tagId);

    public bool HasTag(TagId tagId) =>
        _tagIds.Contains(tagId);
}