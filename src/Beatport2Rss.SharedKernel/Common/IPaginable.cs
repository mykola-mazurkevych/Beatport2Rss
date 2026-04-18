namespace Beatport2Rss.SharedKernel.Common;

public interface IPaginable<out TId>
    where TId : struct, IId<TId>
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}