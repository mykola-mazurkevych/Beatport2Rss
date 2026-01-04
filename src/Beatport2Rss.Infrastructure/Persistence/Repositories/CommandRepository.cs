using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class CommandRepository<TEntity, TId>(Beatport2RssDbContext dbContext) :
    ICommandRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public Task<TEntity> LoadAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.SingleAsync(predicate, cancellationToken);

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        return entities.AsEnumerable();
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        _dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities) =>
        _dbSet.UpdateRange(entities);

    public void Delete(TEntity entity) =>
        _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) =>
        _dbSet.RemoveRange(entities);
}