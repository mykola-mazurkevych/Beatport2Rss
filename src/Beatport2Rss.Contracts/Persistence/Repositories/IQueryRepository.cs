using System.Linq.Expressions;

using Beatport2Rss.SharedKernel;

namespace Beatport2Rss.Contracts.Persistence.Repositories;

public interface IQueryRepository<TEntity, in TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    Task<TEntity?> GetAsync(TId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> NotExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}