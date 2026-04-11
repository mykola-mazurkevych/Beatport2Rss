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

    // public Task<TQueryModel> LoadAsync(
    //     Expression<Func<TQueryModel, bool>> predicate,
    //     CancellationToken cancellationToken = default) =>
    //     queryModels.SingleAsync(
    //         predicate,
    //         cancellationToken);

    // public Task<TQueryModel?> FindAsync(
    //     Expression<Func<TQueryModel, bool>> predicate,
    //     CancellationToken cancellationToken = default) =>
    //     queryModels.SingleOrDefaultAsync(
    //         predicate,
    //         cancellationToken);

    // public async Task<IEnumerable<TQueryModel>> FindAllAsync(
    //     Expression<Func<TQueryModel, bool>> predicate,
    //     CancellationToken cancellationToken = default) =>
    //     (await queryModels.Where(predicate).ToArrayAsync(cancellationToken)).AsEnumerable();

    // protected Task<TModel> LoadAsync<TModel>(
    //     TId id,
    //     Expression<Func<TQueryModel, TModel>> selector,
    //     CancellationToken cancellationToken = default) =>
    //     LoadAsync(
    //         queryModel => queryModel.Id.Equals(id),
    //         selector,
    //         cancellationToken);

    protected Task<TModel> LoadAsync<TModel>(
        Expression<Func<TQueryModel, bool>> predicate,
        Expression<Func<TQueryModel, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        queryModels.Where(predicate).Select(selector).SingleAsync(cancellationToken);

    // protected Task<TModel?> FindAsync<TModel>(
    //     TId id,
    //     Expression<Func<TQueryModel, TModel>> selector,
    //     CancellationToken cancellationToken = default) =>
    //     FindAsync(
    //         queryModel => queryModel.Id.Equals(id),
    //         selector,
    //         cancellationToken);

    protected Task<TModel?> FindAsync<TModel>(
        Expression<Func<TQueryModel, bool>> predicate,
        Expression<Func<TQueryModel, TModel>> selector,
        CancellationToken cancellationToken = default) =>
        queryModels.Where(predicate).Select(selector).SingleOrDefaultAsync(cancellationToken);
}