using System.Linq.Expressions;

using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Querying.Paging;

public interface IPageBuilder
{
    Task<Page<TPageDto>> BuildAsync<TEntity, TId, TPageDto>(
        IQueryable<TEntity> entities,
        Pagination pagination,
        Expression<Func<TEntity, TPageDto>> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : struct, IId<TId>
        where TPageDto : IPageDto<TId>;
}