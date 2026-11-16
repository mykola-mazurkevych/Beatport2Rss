using System.Text.Json.Serialization;

using Beatport2Rss.Common.Beatport.JsonConverters;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportArtistDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] BeatportImageDto Image,
    [property: JsonPropertyName("bio")] string? Bio,
    [property: JsonPropertyName("website")] [property: JsonConverter(typeof(EmptyStringToNullUriJsonConverter))] Uri? Website) :
    BeatportDto(Id, Slug);