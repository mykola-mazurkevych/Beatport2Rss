namespace Beatport2Rss.WebApi.Endpoints.Feeds.Requests;

internal sealed record UpdateFeedRequest(
    string Name,
    bool UpdateSlug,
    bool IsActive);