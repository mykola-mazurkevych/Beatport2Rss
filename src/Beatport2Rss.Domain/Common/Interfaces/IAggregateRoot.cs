namespace Beatport2Rss.Domain.Common.Interfaces;

public interface IAggregateRoot<out TId> : IEntity<TId>
    where TId : struct, IValueObject;