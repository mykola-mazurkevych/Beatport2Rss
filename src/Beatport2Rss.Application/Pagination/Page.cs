namespace Beatport2Rss.Application.Pagination;

public sealed record Page<TDto>(
    IEnumerable<TDto> Dtos,
    PageMetadata Metadata);