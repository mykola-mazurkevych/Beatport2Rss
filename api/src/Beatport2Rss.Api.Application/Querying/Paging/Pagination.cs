namespace Beatport2Rss.Api.Application.Querying.Paging;

public sealed record Pagination(
    int? Size,
    string? Next,
    string? Previous);