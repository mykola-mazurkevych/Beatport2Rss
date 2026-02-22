using Beatport2Rss.Application.Dtos.Tags;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Responses;

internal sealed record GetTagsResponse(
    string Name,
    string Slug)
{
    public static GetTagsResponse Create(TagPageDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value);
}