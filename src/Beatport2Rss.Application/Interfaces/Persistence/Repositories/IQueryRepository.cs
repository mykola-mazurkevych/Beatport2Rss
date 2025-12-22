using System.Linq.Expressions;

using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IQueryRepository<TEntity, in TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    Task<TEntity?> GetAsync(TId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> NotExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}