using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.SharedKernel.Common;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class CommandRepository<TEntity, TId>(DbSet<TEntity> dbSet) :
    ICommandRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IId<TId>
{
    public Task<TEntity> LoadAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        dbSet.SingleAsync(predicate, cancellationToken);

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        dbSet.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        (await dbSet.Where(predicate).ToArrayAsync(cancellationToken)).AsEnumerable();

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        dbSet.AnyAsync(predicate, cancellationToken);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) =>
        dbSet.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities) =>
        dbSet.UpdateRange(entities);

    public void Delete(TEntity entity) =>
        dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) =>
        dbSet.RemoveRange(entities);
}