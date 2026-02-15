using Beatport2Rss.Application.Dtos.Tags;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Responses;

internal sealed record TagResponse(
    string Name,
    string Slug)
{
    public static TagResponse Create(TagDto dto) =>
        new(dto.Name,
            dto.Slug);
}