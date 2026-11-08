using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Api.Domain.Subscriptions;

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
    public Slug Slug { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public Uri ImageUri { get; private set; } = null!;
    public Uri? Website { get; private set; }

    public CountryCode? CountryCode { get; private set; }

    public string? Bio { get; private set; }

    public IReadOnlySet<SubscriptionTag> Tags =>
        _tags.AsReadOnly();

    public static Subscription Create(
        SubscriptionId id,
        DateTimeOffset createdAt,
        SubscriptionType type,
        SubscriptionName name,
        Slug slug,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        Uri imageUri,
        Uri? website,
        CountryCode? countryCode,
        string? bio) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            Name = name,
            Slug = slug,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            ImageUri = imageUri,
            Website = website,
            CountryCode = countryCode,
            Bio = bio,
        };

    public bool ContainsTag(TagId tagId) =>
        _tags.Any(t => t.TagId == tagId);

    public void AddTag(TagId tagId) =>
        _tags.Add(SubscriptionTag.Create(Id, tagId));

    public void RemoveTag(TagId tagId) =>
        _tags.RemoveWhere(t => t.TagId == tagId);

    public void RemoveTags() =>
        _tags.Clear();
}