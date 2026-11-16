using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Builder.Domain.Feeds;

public sealed class RssFeedDocument :
    IAggregateRoot<FeedId>
{
    private RssFeedDocument()
    {
    }

    public FeedId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset GeneratedAt { get; private set; }
    public string Content { get; private set; } = null!;

    public static RssFeedDocument Create(
        FeedId id,
        DateTimeOffset createdAt,
        DateTimeOffset generatedAt,
        string content) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            GeneratedAt = generatedAt,
            Content = content,
        };

    public void UpdateContent(
        DateTimeOffset generatedAt,
        string content)
    {
        GeneratedAt = generatedAt;
        Content = content;
    }
}