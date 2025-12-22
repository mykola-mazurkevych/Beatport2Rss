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

    public TagName Name { get; private set; }
    public Slug Slug { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public UserId UserId { get; private set; }

    public static Tag Create(
        TagId id,
        TagName name,
        Slug slug,
        DateTimeOffset createdDate,
        UserId userId) =>
        new()
        {
            Id = id,
            Name = name,
            Slug = slug,
            CreatedDate = createdDate,
            UserId = userId,
        };
}