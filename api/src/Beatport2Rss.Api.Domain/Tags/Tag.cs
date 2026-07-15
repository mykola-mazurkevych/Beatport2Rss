using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Domain.Tags;

public sealed class Tag :
    IAggregateRoot<TagId>
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
        TagId id,
        DateTimeOffset createdAt,
        UserId userId,
        TagName name,
        Slug slug) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
            Name = name,
            Slug = slug,
        };

    public void UpdateName(TagName name) =>
        Name = name;

    public void UpdateSlug(Slug slug) =>
        Slug = slug;
}