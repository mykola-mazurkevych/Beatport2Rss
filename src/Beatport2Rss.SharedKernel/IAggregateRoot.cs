namespace Beatport2Rss.SharedKernel;

public interface IAggregateRoot<out TId> : IEntity<TId>
    where TId : struct, IValueObject;