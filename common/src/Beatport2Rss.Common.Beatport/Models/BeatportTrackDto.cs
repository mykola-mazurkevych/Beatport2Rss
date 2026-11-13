using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportTrackDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("mix_name")] string MixName,
    [property: JsonPropertyName("length_ms")] int LengthMilliseconds,
    [property: JsonPropertyName("sample_url")] Uri SampleUri,
    [property: JsonPropertyName("artists")] IReadOnlyCollection<BeatportReleaseArtistDto> Artists,
    [property: JsonPropertyName("remixers")] IReadOnlyCollection<BeatportReleaseArtistDto> Remixers) :
    BeatportDto(Id, Slug);