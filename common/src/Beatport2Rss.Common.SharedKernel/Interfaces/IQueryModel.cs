#pragma warning disable CA1040 // Avoid empty interfaces

namespace Beatport2Rss.Common.SharedKernel.Interfaces;

public interface IQueryModel<out TId>
    where TId : struct, IId<TId>;