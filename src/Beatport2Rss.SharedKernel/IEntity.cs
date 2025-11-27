namespace Beatport2Rss.SharedKernel;

public interface IEntity<out TId>
    where TId : struct, IValueObject
{
    TId Id { get; }
}