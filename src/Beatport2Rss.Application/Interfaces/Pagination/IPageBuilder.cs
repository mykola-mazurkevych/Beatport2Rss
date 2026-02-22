#pragma warning disable CA1716 // Identifiers should not match keywords

using System.Linq.Expressions;

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Pagination;

public interface IPageBuilder
{
    Task<Page<TPageDto>> BuildAsync<TEntity, TId, TPageDto>(
        IQueryable<TEntity> entities,
        int? size,
        string? next,
        string? previous,
        Expression<Func<TEntity, TPageDto>> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : struct, IId<TId>
        where TPageDto : IPageDto<TId>;
}