namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportReleaseArtistDto(
    int Id,
    string Slug) :
    BeatportDto(Id, Slug);