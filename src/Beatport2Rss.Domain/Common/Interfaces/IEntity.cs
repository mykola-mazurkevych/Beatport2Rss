namespace Beatport2Rss.Domain.Common.Interfaces;

public interface IEntity<out TId>
    where TId : struct, IValueObject
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}