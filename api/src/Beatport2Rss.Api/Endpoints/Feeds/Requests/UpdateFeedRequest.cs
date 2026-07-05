namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record UpdateFeedRequest(
    string Name,
    bool UpdateSlug,
    bool IsActive);