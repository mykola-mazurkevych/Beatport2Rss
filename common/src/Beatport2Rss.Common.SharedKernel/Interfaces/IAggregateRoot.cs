namespace Beatport2Rss.Common.SharedKernel.Interfaces;

public interface IAggregateRoot<out TId> :
    IEntity<TId>
    where TId : struct, IId<TId>;