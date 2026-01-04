using System.Linq.Expressions;

using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ICommandRepository<TEntity, in TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    Task<TEntity> LoadAsync(TId id, CancellationToken cancellationToken = default) =>
        LoadAsync(e => e.Id.Equals(id), cancellationToken);

    Task<TEntity> LoadAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity?> FindAsync(TId id, CancellationToken cancellationToken = default) =>
        FindAsync(e => e.Id.Equals(id), cancellationToken);

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}