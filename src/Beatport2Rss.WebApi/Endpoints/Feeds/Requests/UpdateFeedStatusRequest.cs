namespace Beatport2Rss.WebApi.Endpoints.Feeds.Requests;

internal sealed record UpdateFeedStatusRequest(
    bool IsActive);