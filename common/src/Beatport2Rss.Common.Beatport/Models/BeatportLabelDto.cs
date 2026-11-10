using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportLabelDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] BeatportImageDto Image) :
    BeatportDto(Id, Slug);