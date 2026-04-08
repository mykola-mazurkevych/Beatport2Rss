namespace Beatport2Rss.SharedKernel.Common;

public interface IQueryModel<out TId>
    where TId : struct, IId<TId>
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}