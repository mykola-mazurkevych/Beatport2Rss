using Beatport2Rss.Api.Application.Dtos.Tags;

namespace Beatport2Rss.Api.Endpoints.Tags.Responses;

internal sealed record TagResponse(
    int Id,
    string Name,
    string Slug,
    DateTimeOffset CreatedAt)
{
    public static TagResponse Create(TagDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.CreatedAt);
}