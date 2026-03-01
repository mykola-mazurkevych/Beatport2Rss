using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Application.Dtos.Beatport;

public abstract record BeatportDto(
    BeatportId Id,
    BeatportSlug Slug);