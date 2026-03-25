namespace Beatport2Rss.Application.Pagination;

public sealed record PageMetadata(
    int PageSize,
    int Count,
    int TotalCount,
    string? NextPage,
    string? PreviousPage)
{
    public int PagesCount => (int)Math.Ceiling((double)TotalCount / PageSize);

    public static PageMetadata Empty(int pageSize) =>
        new(pageSize,
            Count: 0,
            TotalCount: 0,
            NextPage: null,
            PreviousPage: null);
}