using System.Linq.Expressions;

using Beatport2Rss.SharedKernel.Common;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class QueryRepository<TQueryModel, TId>(
    IQueryable<TQueryModel> queryModels)
    where TQueryModel : IQueryModel<TId>
    where TId : struct, IId<TId>
{
    public Task<bool> ExistsAsync(
        Expression<Func<TQueryModel, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        queryModels.AnyAsync(
            predicate,
            cancellationToken);

    protected Task<TModel> LoadAsync<TModel>(
        Expression<Func<TQueryModel, bool>> predicate,
        Expression<Func<TQueryModel, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        queryModels.Where(predicate).Select(selector).SingleAsync(cancellationToken);

    protected Task<TModel?> FindAsync<TModel>(
        Expression<Func<TQueryModel, bool>> predicate,
        Expression<Func<TQueryModel, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        queryModels.Where(predicate).Select(selector).SingleOrDefaultAsync(cancellationToken);
}