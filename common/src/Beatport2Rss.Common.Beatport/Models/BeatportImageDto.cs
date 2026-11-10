using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportImageDto(
    [property: JsonPropertyName("uri")] Uri Uri);