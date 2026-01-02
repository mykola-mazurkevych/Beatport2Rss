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

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TEntity entity) =>
        _dbSet.Update(entity);

    public void Delete(TEntity entity) =>
        _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) =>
        _dbSet.RemoveRange(entities);
}