using Beatport2Rss.Application.Pagination;

namespace Beatport2Rss.WebApi.Endpoints;

internal sealed record PageResponse<TResponse>(
    int Size,
    IEnumerable<TResponse> Items,
    int TotalCount,
    string? Next,
    string? Previous)
{
    public static PageResponse<TResponse> Create<TPageDto>(
        Page<TPageDto> page,
        Func<TPageDto, TResponse> selector) =>
        new(page.Size,
            page.Items.Select(selector),
            page.TotalCount,
            page.Next,
            page.Previous);
}