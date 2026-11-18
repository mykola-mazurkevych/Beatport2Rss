using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportGenreDto(
    int Id,
    string Slug,
    [property: JsonPropertyName("name")] string Name) :
    BeatportDto(Id, Slug);
