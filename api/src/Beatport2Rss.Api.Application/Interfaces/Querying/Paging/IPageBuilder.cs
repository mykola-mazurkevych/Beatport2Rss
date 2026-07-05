using Beatport2Rss.Api.Application.Querying.Paging;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Application.Interfaces.Querying.Paging;

public interface IPageBuilder
{
    Task<Page<TPaginable>> BuildAsync<TPaginable, TId>(
        IQueryable<TPaginable> paginables,
        Pagination pagination,
        CancellationToken cancellationToken = default)
        where TPaginable : class, IPaginable<TId>
        where TId : struct, IId<TId>;
}