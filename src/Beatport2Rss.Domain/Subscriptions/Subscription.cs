using Beatport2Rss.SharedKernel;
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
    public long BeatportId { get; private set; }
    public string BeatportSlug { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Uri ImageUri { get; private set; } = null!;

    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset? PulledDate { get; private set; }

    public IReadOnlySet<FeedId> FeedIds => _feedIds.AsReadOnly();
    public IReadOnlySet<TagId> TagIds => _tagIds.AsReadOnly();

    public static Subscription Create(
        SubscriptionId id,
        BeatportEntityType beatportType,
        string beatportSlug,
        long beatportId,
        string name,
        Uri imageUri) =>
        Create(id, beatportType, beatportId, beatportSlug, name, imageUri, DateTimeOffset.UtcNow, pulledDate: null);

    public static Subscription Create(
        SubscriptionId id,
        BeatportEntityType beatportType,
        long beatportId,
        string beatportSlug,
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

    public void AddTag(TagId tagId) => _tagIds.Add(tagId);
    public void RemoveTag(TagId tagId) => _tagIds.Remove(tagId);
    public bool HasTag(TagId tagId) => _tagIds.Contains(tagId);
}