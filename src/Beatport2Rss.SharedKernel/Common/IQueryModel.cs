#pragma warning disable CA1040 // Avoid empty interfaces

namespace Beatport2Rss.SharedKernel.Common;

public interface IQueryModel<out TId>
    where TId : struct, IId<TId>;