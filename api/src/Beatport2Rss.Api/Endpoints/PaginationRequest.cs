namespace Beatport2Rss.Api.Endpoints;

internal sealed record PaginationRequest(
    int? Size,
    string? Next,
    string? Previous);