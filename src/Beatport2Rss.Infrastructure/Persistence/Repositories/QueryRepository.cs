using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal abstract class QueryRepository<TEntity, TId>(Beatport2RssDbContext dbContext) :
    IQueryRepository<TEntity, TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    private IQueryable<TEntity> Entities => ApplyIncludes(dbContext.Set<TEntity>()).AsNoTracking();

    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> entities) => entities;

    public Task<TEntity> LoadAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Entities.SingleAsync(predicate, cancellationToken);

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Entities.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await Entities.Where(predicate).ToListAsync(cancellationToken);

        return entities.AsEnumerable();
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Entities.AnyAsync(predicate, cancellationToken);

    public async Task<bool> NotExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        !(await ExistsAsync(predicate, cancellationToken));
}