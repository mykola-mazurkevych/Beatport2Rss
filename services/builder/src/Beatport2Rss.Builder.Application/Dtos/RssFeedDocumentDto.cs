namespace Beatport2Rss.Builder.Application.Dtos;

public enum RssFeedDocumentStatus
{
    NotFound,
    NotModified,
    Found,
}

public sealed record RssFeedDocumentDto(
    RssFeedDocumentStatus Status,
    DateTimeOffset? GeneratedAt,
    string? Content)
{
    public static RssFeedDocumentDto NotFound() =>
        new(RssFeedDocumentStatus.NotFound, null, null);

    public static RssFeedDocumentDto NotModified(DateTimeOffset generatedAt) =>
        new(RssFeedDocumentStatus.NotModified, generatedAt, null);

    public static RssFeedDocumentDto Found(DateTimeOffset generatedAt, string content) =>
        new(RssFeedDocumentStatus.Found, generatedAt, content);
}
