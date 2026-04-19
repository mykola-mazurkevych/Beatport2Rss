using Beatport2Rss.Application.Dtos.Tags;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Responses;

internal sealed record TagPaginableResponse(
    string Name,
    string Slug)
{
    public static TagPaginableResponse Create(TagPaginableDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value);
}