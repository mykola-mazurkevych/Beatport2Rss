using Beatport2Rss.Api.Domain.Common.ValueObjects;
using Beatport2Rss.Api.Domain.Countries;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Common.SharedKernel.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

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

    public CountryCode? CountryCode { get; private set; }

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
        CountryCode? countryCode) =>
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
            CountryCode = countryCode,
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