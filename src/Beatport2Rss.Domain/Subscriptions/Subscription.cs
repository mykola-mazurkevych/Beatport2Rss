// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Countries;
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

    public SubscriptionName Name { get; private set; }
    public Slug Slug { get; private set; }

    public BeatportSubscriptionType BeatportType { get; private set; }
    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public Uri ImageUri { get; private set; } = null!;

    public DateTimeOffset? RefreshedAt { get; private set; }

    public CountryCode? CountryCode { get; private set; }

    public IReadOnlySet<SubscriptionTag> Tags =>
        _tags.AsReadOnly();

    public static Subscription Create(
        DateTimeOffset createdAt,
        SubscriptionName name,
        Slug slug,
        BeatportSubscriptionType beatportType,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        Uri imageUri,
        DateTimeOffset? refreshedAt,
        CountryCode? countryCode) =>
        new()
        {
            CreatedAt = createdAt,
            Name = name,
            Slug = slug,
            BeatportType = beatportType,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            ImageUri = imageUri,
            RefreshedAt = refreshedAt,
            CountryCode = countryCode,
        };

    public void MarkAsRefreshed(DateTimeOffset refreshedAt) =>
        RefreshedAt = refreshedAt;

    public bool ContainsTag(TagId tagId) =>
        _tags.Any(t => t.TagId == tagId);

    public void AddTag(TagId tagId) =>
        _tags.Add(SubscriptionTag.Create(Id, tagId));

    public void RemoveTag(TagId tagId) =>
        _tags.RemoveWhere(t => t.TagId == tagId);

    public void RemoveTags() =>
        _tags.Clear();
}