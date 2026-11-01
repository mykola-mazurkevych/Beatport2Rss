namespace Beatport2Rss.Common.Domain.Interfaces;

public interface IAggregateRoot<out TId> :
    IEntity<TId>
    where TId : struct, IId<TId>;