using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Common.SharedKernel.Interfaces;

public interface IPaginable<out TId>
    where TId : struct, IId<TId>
{
    TId Id { get; }

    DateTimeOffset CreatedAt { get; }
}