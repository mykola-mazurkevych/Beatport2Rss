namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record UpdateFeedRequest(
    string Name,
    string? AuthorName,
    bool UpdateSlug,
    bool IsActive);