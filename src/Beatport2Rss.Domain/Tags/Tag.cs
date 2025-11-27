using Beatport2Rss.SharedKernel;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Domain.Tags;

public sealed class Tag : IEntity<TagId>
{
    private Tag()
    {
    }

    public TagId Id { get; private set; }

    public TagName Name { get; private set; }
    public string Slug { get; private set; } = null!;

    public DateTimeOffset CreatedDate { get; private set; }

    public UserId UserId { get; private set; }

    public static Tag Create(
        TagId id,
        TagName name,
        string slug,
        UserId userId) =>
        Create(id, name, slug, DateTimeOffset.UtcNow, userId);

    public static Tag Create(
        TagId id,
        TagName name,
        string slug,
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