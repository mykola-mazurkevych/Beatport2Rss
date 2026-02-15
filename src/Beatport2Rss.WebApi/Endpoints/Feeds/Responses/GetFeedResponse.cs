using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

public sealed record GetFeedResponse(
    Guid Id,
    string Slug,
    string Name,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt)
{
    public static GetFeedResponse Create(FeedDto dto) =>
        new(dto.Id,
            dto.Slug,
            dto.Name,
            dto.Owner,
            dto.IsActive,
            dto.CreatedAt);
}