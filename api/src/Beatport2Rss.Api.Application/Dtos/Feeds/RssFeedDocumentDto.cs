namespace Beatport2Rss.Api.Application.Dtos.Feeds;

public sealed record RssFeedDocumentDto
{
    public required string Content { get; init; }
}