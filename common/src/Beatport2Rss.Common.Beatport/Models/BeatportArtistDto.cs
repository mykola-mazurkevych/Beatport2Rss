namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportArtistDto(
    int Id,
    string Slug,
    string Name,
    BeatportImage Image,
    string? Bio,
    Uri? Website) :
    BeatportDto(Id, Slug);