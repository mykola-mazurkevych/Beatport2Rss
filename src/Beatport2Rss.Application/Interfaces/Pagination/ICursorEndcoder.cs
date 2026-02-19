using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Application.Interfaces.Pagination;

public interface ICursorEndcoder
{
    string Encode<TId>(Cursor<TId> cursor)
        where TId : struct, IValueObject;

    Cursor<TId>? Decode<TId>(string? base64String)
        where TId : struct, IValueObject;
}