using System.Linq.Expressions;

using Beatport2Rss.SharedKernel.Common;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class CommandRepository<TAggregateRoot, TId>(DbSet<TAggregateRoot> dbSet)
    where TAggregateRoot : class, IAggregateRoot<TId>
    where TId : struct, IId<TId>
{
    public Task<TAggregateRoot> LoadAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        dbSet.SingleAsync(predicate, cancellationToken);

    public Task<TAggregateRoot?> FindAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        dbSet.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TAggregateRoot>> FindAllAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        (await dbSet.Where(predicate).ToListAsync(cancellationToken)).AsEnumerable();

    public Task<bool> ExistsAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        dbSet.AnyAsync(predicate, cancellationToken);

    public async Task<TAggregateRoot> AddAsync(
        TAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default) =>
        (await dbSet.AddAsync(aggregateRoot, cancellationToken)).Entity;

    public void Update(TAggregateRoot aggregateRoot) =>
        dbSet.Update(aggregateRoot);

    public void Delete(TAggregateRoot aggregateRoot) =>
        dbSet.Remove(aggregateRoot);

    public Task DeleteAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
}