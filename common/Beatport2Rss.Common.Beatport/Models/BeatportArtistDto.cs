namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportArtistDto(
    int Id,
    string Slug,
    string Name,
    BeatportImage Image) :
    BeatportDto(Id, Slug);