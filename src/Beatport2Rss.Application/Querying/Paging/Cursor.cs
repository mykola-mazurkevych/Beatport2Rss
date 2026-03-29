using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Querying.Paging;

public sealed record Cursor<TId>(
    DateTimeOffset CreatedAt,
    TId Id)
    where TId : struct, IId<TId>;