using System.Linq.Expressions;

using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ICommandRepository<TAggregateRoot, in TId>
    where TAggregateRoot : class, IAggregateRoot<TId>
    where TId : struct, IId<TId>
{
    Task<TAggregateRoot> LoadAsync(
        TId id,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            aggregateRoot => aggregateRoot.Id.Equals(id),
            cancellationToken);

    Task<TAggregateRoot> LoadAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TAggregateRoot?> FindAsync(
        TId id,
        CancellationToken cancellationToken = default) =>
        FindAsync(
            aggregateRoot => aggregateRoot.Id.Equals(id),
            cancellationToken);

    Task<TAggregateRoot?> FindAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TAggregateRoot>> FindAllAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TAggregateRoot> AddAsync(
        TAggregateRoot aggregateRoot,
        CancellationToken cancellationToken = default);

    void Update(TAggregateRoot aggregateRoot);

    void Delete(TAggregateRoot aggregateRoot);

    Task DeleteAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default);
}