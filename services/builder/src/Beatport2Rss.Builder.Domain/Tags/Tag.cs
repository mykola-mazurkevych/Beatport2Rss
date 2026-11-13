using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Builder.Domain.Tags;

public sealed class Tag :
    IAggregateRoot<TagId>
{
    private Tag()
    {
    }

    public TagId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public TagName Name { get; private set; }

    public static Tag Create(
        TagId id,
        DateTimeOffset createdAt,
        TagName name) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Name = name,
        };

    public void UpdateName(TagName name) =>
        Name = name;
}