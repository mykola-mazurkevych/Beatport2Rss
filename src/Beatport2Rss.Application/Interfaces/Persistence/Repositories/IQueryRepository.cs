#pragma warning disable CA1040 // Avoid empty interfaces

using System.Linq.Expressions;

using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IQueryRepository;

public interface IQueryRepository<TQueryModel, in TId>
    where TQueryModel : IQueryModel<TId>
    where TId : struct, IId<TId>
{
    Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            queryModel => queryModel.Id.Equals(id),
            cancellationToken);

    Task<bool> ExistsAsync(
        Expression<Func<TQueryModel, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TQueryModel> LoadAsync(
        TId id,
        CancellationToken cancellationToken = default) =>
        LoadAsync(
            queryModel => queryModel.Id.Equals(id),
            cancellationToken);

    Task<TQueryModel> LoadAsync(
        Expression<Func<TQueryModel, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<TQueryModel?> FindAsync(
        TId id,
        CancellationToken cancellationToken = default) =>
        FindAsync(
            queryModel => queryModel.Id.Equals(id),
            cancellationToken);

    Task<TQueryModel?> FindAsync(
        Expression<Func<TQueryModel, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TQueryModel>> FindAllAsync(
        Expression<Func<TQueryModel, bool>> predicate,
        CancellationToken cancellationToken = default);
}