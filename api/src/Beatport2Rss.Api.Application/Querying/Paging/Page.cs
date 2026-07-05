namespace Beatport2Rss.Api.Application.Querying.Paging;

public sealed record Page<TDto>(
    IEnumerable<TDto> Dtos,
    PageInfo Info);