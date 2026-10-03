using System.Linq.Expressions;

using Beatport2Rss.Common.SharedKernel.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

public abstract class CommandRepository<TEntity>(DbSet<TEntity> entities)
    where TEntity : class
{
    public Task<TEntity> LoadAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        entities.SingleAsync(predicate, cancellationToken);

    public Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        entities.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        (await entities.Where(predicate).ToListAsync(cancellationToken)).AsEnumerable();

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        entities.AnyAsync(predicate, cancellationToken);

    public async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default) =>
        (await entities.AddAsync(entity, cancellationToken)).Entity;

    public void Update(TEntity entity) =>
        entities.Update(entity);

    public void Delete(TEntity entity) =>
        entities.Remove(entity);

    public Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        entities.Where(predicate).ExecuteDeleteAsync(cancellationToken);
}

public abstract class CommandRepository<TAggregateRoot, TId>(DbSet<TAggregateRoot> dbSet) :
    CommandRepository<TAggregateRoot>(dbSet)
    where TAggregateRoot : class, IAggregateRoot<TId>
    where TId : struct, IId<TId>;