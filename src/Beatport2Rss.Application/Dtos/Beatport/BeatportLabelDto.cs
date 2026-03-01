using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Application.Dtos.Beatport;

public sealed record BeatportLabelDto (
    BeatportId Id,
    BeatportSlug Slug,
    string Name,
    BeatportImage Image) :
    BeatportDto(Id, Slug);