namespace Beatport2Rss.Application.Pagination;

public sealed record Page<TItem>
{
    public int Size { get; init; }

    public IReadOnlyCollection<TItem> Items { get; init; } = [];

    public int Count => Items.Count;
    public int TotalCount { get; init; }

    public string? Next { get; init; }
    public string? Previous { get; init; }
}