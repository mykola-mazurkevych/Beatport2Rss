using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public abstract record BeatportDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("slug")] string Slug);