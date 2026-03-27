namespace Beatport2Rss.Application.Pagination;

public sealed record PageNavigation(
    int? Size,
    string? Next,
    string? Previous);