namespace Beatport2Rss.Common.Beatport.Models;

public sealed record BeatportReleaseLabelDto(
    int Id,
    string Slug) :
    BeatportDto(Id, Slug);