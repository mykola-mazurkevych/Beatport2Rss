using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

internal sealed record FeedPageResponse(
    string Name,
    string Slug,
    bool IsActive,
    int SubscriptionsCount)
{
    public static FeedPageResponse Create(FeedPageDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value,
            dto.IsActive,
            dto.SubscriptionsCount);
}