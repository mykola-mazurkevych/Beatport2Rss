namespace Beatport2Rss.SharedKernel.Common;

public interface IAggregateRoot<out TId> :
    IEntity<TId>
    where TId : struct, IId<TId>;