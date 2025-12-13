using Beatport2Rss.SharedKernel;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class CommandRepository<TEntity, TId>(Beatport2RssDbContext dbContext)
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Delete(TEntity entity) =>
        _dbSet.Remove(entity);
}