using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportReleaseDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("artists")] IReadOnlyCollection<BeatportReleaseArtistDto> Artists,
    [property: JsonPropertyName("remixers")] IReadOnlyCollection<BeatportReleaseArtistDto> Remixers,
    [property: JsonPropertyName("label")] BeatportReleaseLabelDto Label,
    [property: JsonPropertyName("catalog_number")] string CatalogNumber,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("desc")] string? Description,
    [property: JsonPropertyName("new_release_date")] DateOnly NewReleaseDate,
    [property: JsonPropertyName("publish_date")] DateOnly PublishDate,
    [property: JsonPropertyName("image")] BeatportImageDto Image,
    [property: JsonPropertyName("track_count")]int TracksCount,
    [property: JsonPropertyName("tracks")] IReadOnlyCollection<Uri> TrackUris) :
    BeatportDto(Id, Slug);