using Beatport2Rss.SharedKernel;

namespace Beatport2Rss.Contracts.Repositories;

public interface ICommandRepository<in TEntity, TId>
    where TEntity : class, IAggregateRoot<TId>
    where TId : struct, IValueObject
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    void Update(TEntity entity);
    
    void Delete(TEntity entity);
}