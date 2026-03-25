using Beatport2Rss.Application.Dtos.Tags;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Responses;

internal sealed record TagPageResponse(
    string Name,
    string Slug)
{
    public static TagPageResponse Create(TagPageDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value);
}