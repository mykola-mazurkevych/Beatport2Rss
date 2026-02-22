using Beatport2Rss.Application.Dtos.Feeds;

namespace Beatport2Rss.WebApi.Endpoints.Feeds.Responses;

internal sealed record GetFeedsResponse(
    string Name,
    string Slug,
    bool IsActive)
{
    public static GetFeedsResponse Create(FeedPageDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value,
            dto.IsActive);
}