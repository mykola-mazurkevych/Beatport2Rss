// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Domain.Tags;

public sealed class Tag : IEntity<TagId>
{
    private Tag()
    {
    }

    public TagId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public UserId UserId { get; private set; }

    public TagName Name { get; private set; }
    public Slug Slug { get; private set; }

    public static Tag Create(
        DateTimeOffset createdAt,
        UserId userId,
        TagName name,
        Slug slug) =>
        new()
        {
            CreatedAt = createdAt,
            UserId = userId,
            Name = name,
            Slug = slug,
        };

    internal void UpdateName(TagName name) =>
        Name = name;

    internal void UpdateSlug(Slug slug) =>
        Slug = slug;
}