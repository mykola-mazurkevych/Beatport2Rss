using System.Linq.Expressions;

using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IQueryRepository<TEntity, in TId>
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

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    async Task<bool> NotExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        !(await ExistsAsync(predicate, cancellationToken));
}