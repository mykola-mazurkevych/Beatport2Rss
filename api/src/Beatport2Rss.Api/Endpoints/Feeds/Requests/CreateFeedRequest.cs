namespace Beatport2Rss.Api.Endpoints.Feeds.Requests;

internal sealed record CreateFeedRequest(
    string Name,
    bool IsActive);