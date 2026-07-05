using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Application.Querying.Paging;

public sealed record Cursor<TId>(
    DateTimeOffset CreatedAt,
    TId Id)
    where TId : struct, IId<TId>;