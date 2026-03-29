namespace Beatport2Rss.Application.Querying.Paging;

public sealed record PageInfo(
    int Size,
    int ItemsCount,
    int TotalItemsCount,
    string? Next,
    string? Previous)
{
    public int TotalCount => (int)Math.Ceiling((double)TotalItemsCount / Size);

    public static PageInfo Empty(int size) =>
        new(size,
            ItemsCount: 0,
            TotalItemsCount: 0,
            Next: null,
            Previous: null);
}