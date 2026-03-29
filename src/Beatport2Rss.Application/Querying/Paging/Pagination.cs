namespace Beatport2Rss.Application.Querying.Paging;

public sealed record Pagination(
    int? Size,
    string? Next,
    string? Previous);