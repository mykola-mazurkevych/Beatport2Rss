namespace Beatport2Rss.WebApi.Endpoints;

internal sealed record PaginationRequest(
    int? Size,
    string? Next,
    string? Previous);