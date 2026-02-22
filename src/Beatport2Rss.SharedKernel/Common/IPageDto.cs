namespace Beatport2Rss.SharedKernel.Common;

public interface IPageDto<out TId>
    where TId : IId<TId>
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}