#pragma warning disable CA1716 // Identifiers should not match keywords

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Pagination;

public interface IPageBuilder
{
    Task<Page<TDto>> BuildAsync<TEntity, TId, TDto>(
        IQueryable<TEntity> entitiesAsQueryable,
        int? size,
        string? next,
        string? previous,
        Func<TEntity, TDto> dtoSelector,
        CancellationToken cancellationToken = default)
        where TEntity : IEntity<TId>
        where TId : struct, IId<TId>;
}