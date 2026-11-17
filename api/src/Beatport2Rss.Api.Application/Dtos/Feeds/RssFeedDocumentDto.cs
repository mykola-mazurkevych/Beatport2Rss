namespace Beatport2Rss.Api.Application.Dtos.Feeds;

public sealed record RssFeedDocumentDto
{
    public required DateTimeOffset LastModified { get; init; }
    public required string Content { get; init; }
}