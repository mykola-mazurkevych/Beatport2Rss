namespace Beatport2Rss.WebApi.Endpoints.Feeds.Requests;

public sealed record UpdateFeedStatusRequest(
    bool IsActive);