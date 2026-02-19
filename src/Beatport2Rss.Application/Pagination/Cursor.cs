using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Application.Pagination;

public sealed record Cursor<TId>(DateTimeOffset CreatedAt, TId Id)
    where TId : struct, IValueObject;