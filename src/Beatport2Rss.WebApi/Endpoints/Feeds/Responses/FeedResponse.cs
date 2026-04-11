using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

internal sealed record FeedResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount)
{
    public static FeedResponse Create(FeedDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.IsActive,
            dto.CreatedAt,
            dto.SubscriptionsCount);
}