namespace Beatport2Rss.Common.Domain.Interfaces;

public interface IEntity<out TId>
    where TId : struct, IId<TId>
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}