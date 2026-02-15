using Beatport2Rss.Application.Dtos.Tags;

namespace Beatport2Rss.WebApi.Endpoints.Tags.Responses;

internal sealed record TagResponse(
    int Id,
    string Name,
    string Slug,
    DateTimeOffset CreatedAt)
{
    public static TagResponse Create(TagDto dto) =>
        new(dto.Id,
            dto.Name,
            dto.Slug,
            dto.CreatedAt);
}