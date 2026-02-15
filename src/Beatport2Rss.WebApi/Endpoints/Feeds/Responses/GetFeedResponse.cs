using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

internal sealed record GetFeedResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Owner,
    bool IsActive,
    DateTimeOffset CreatedAt)
{
    public static GetFeedResponse Create(FeedDto dto) =>
        new(dto.Id,
            dto.Name,
            dto.Slug,
            dto.Owner,
            dto.IsActive,
            dto.CreatedAt);
}