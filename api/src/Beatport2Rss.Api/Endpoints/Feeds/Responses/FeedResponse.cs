using Beatport2Rss.Api.Application.Dtos.Feeds;

namespace Beatport2Rss.Api.Endpoints.Feeds.Responses;

internal sealed record FeedResponse(
    Guid Id,
    string Name,
    string Slug,
    string? AuthorName,
    bool IsActive,
    DateTimeOffset CreatedAt,
    int SubscriptionsCount)
{
    public static FeedResponse Create(FeedDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.AuthorName?.Value,
            dto.IsActive,
            dto.CreatedAt,
            dto.SubscriptionsCount);
}