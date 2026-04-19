using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

internal sealed record FeedPaginableResponse(
    string Name,
    string Slug,
    bool IsActive,
    int SubscriptionsCount)
{
    public static FeedPaginableResponse Create(FeedPaginableDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value,
            dto.IsActive,
            dto.SubscriptionsCount);
}