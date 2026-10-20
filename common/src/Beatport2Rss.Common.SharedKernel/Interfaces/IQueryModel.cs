#pragma warning disable CA1040 // Avoid empty interfaces

using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Common.SharedKernel.Interfaces;

public interface IQueryModel<out TId>
    where TId : struct, IId<TId>;