using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Domain.Subscriptions;

public sealed class Subscription : IAggregateRoot<SubscriptionId>
{
    private readonly HashSet<FeedId> _feedIds = [];
    private readonly HashSet<TagId> _tagIds = [];

    private Subscription()
    {
    }

    public SubscriptionId Id { get; private set; }

    public BeatportEntityType BeatportType { get; private set; }
    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public string Name { get; private set; } = null!;

    public Uri ImageUri { get; private set; } = null!;

    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset? PulledDate { get; private set; }

    public IReadOnlySet<FeedId> FeedIds => _feedIds.AsReadOnly();
    public IReadOnlySet<TagId> TagIds => _tagIds.AsReadOnly();

    public static Subscription Create(
        SubscriptionId id,
        BeatportEntityType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        string name,
        Uri imageUri,
        DateTimeOffset createdDate,
        DateTimeOffset? pulledDate) =>
        new()
        {
            Id = id,
            BeatportType = beatportType,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Name = name,
            ImageUri = imageUri,
            CreatedDate = createdDate,
            PulledDate = pulledDate,
        };

    public void MarkAsPulled(DateTimeOffset pulledDate) => PulledDate = pulledDate;

    public void AddTag(Tag tag) => _tagIds.Add(tag.Id);
    public void RemoveTag(Tag tag) => _tagIds.Remove(tag.Id);
    public bool HasTag(Tag tag) => _tagIds.Contains(tag.Id);
}