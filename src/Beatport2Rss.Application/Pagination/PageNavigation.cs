namespace Beatport2Rss.Application.Pagination;

public sealed record PageNavigation(
    int? PageSize,
    string? NextPage,
    string? PreviousPage);