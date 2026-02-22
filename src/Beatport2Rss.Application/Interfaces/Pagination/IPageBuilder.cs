#pragma warning disable CA1716 // Identifiers should not match keywords

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Pagination;

public interface IPageBuilder
{
    Task<Page<TListDto>> BuildAsync<TListDto, TId>(
        IQueryable<TListDto> entitiesAsQueryable,
        int? size,
        string? next,
        string? previous,
        CancellationToken cancellationToken = default)
        where TListDto : class, IPageDto<TId>
        where TId : struct, IId<TId>;
}