namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record UpdateFeedStatusRequest(
    bool IsActive);