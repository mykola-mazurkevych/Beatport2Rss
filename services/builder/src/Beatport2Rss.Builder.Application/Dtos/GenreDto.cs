using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Builder.Application.Dtos;

public sealed record GenreDto(
    BeatportId BeatportId,
    string Name);
