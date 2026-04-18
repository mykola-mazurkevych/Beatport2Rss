using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Querying.Paging;

public interface IPageBuilder
{
    Task<Page<TPaginable>> BuildAsync<TPaginable, TId>(
        IQueryable<TPaginable> paginables,
        Pagination pagination,
        CancellationToken cancellationToken = default)
        where TPaginable : class, IPaginable<TId>
        where TId : struct, IId<TId>;
}