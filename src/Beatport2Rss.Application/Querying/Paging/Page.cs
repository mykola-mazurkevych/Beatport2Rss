namespace Beatport2Rss.Application.Querying.Paging;

public sealed record Page<TDto>(
    IEnumerable<TDto> Dtos,
    PageInfo Info);