using System.Linq.Expressions;

using Beatport2Rss.Contracts.Persistence.Repositories;
using Beatport2Rss.SharedKernel;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class QueryRepository<TEntity, TId>(Beatport2RssDbContext dbContext)
    : IQueryRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    protected abstract IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query);

    public Task<TEntity> GetAsync(TId id, CancellationToken cancellationToken = default) =>
        ApplyIncludes(_dbSet).SingleAsync(entity => entity.Id.Equals(id), cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.AnyAsync(predicate, cancellationToken);

    public Task<bool> NotExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.AnyAsync(predicate, cancellationToken)
            .ContinueWith(t => !t.Result, TaskScheduler.Current);
}