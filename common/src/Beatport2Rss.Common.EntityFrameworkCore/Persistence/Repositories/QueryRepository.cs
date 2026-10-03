using System.Linq.Expressions;

using Beatport2Rss.Common.SharedKernel.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

public abstract class QueryRepository<TEntity>(IQueryable<TEntity> entities)
{
    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        entities.AnyAsync(predicate, cancellationToken);

    protected Task<TModel> LoadAsync<TModel>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        entities.Where(predicate).Select(selector).SingleAsync(cancellationToken);

    protected Task<TModel?> FindAsync<TModel>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        entities.Where(predicate).Select(selector).SingleOrDefaultAsync(cancellationToken);
}

public abstract class QueryRepository<TQueryModel, TId>(IQueryable<TQueryModel> queryModels) :
    QueryRepository<TQueryModel>(queryModels)
    where TQueryModel : IQueryModel<TId>
    where TId : struct, IId<TId>;