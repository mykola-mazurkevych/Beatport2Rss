using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportArtistDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] BeatportImageDto Image,
    [property: JsonPropertyName("bio")] string? Bio,
    [property: JsonPropertyName("website")] Uri? Website) :
    BeatportDto(Id, Slug);