using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Interfaces.Querying.Paging;

public interface ICursorEncoder
{
    string Encode<TId>(Cursor<TId> cursor)
        where TId : struct, IId<TId>;

    Cursor<TId>? Decode<TId>(string? base64String)
        where TId : struct, IId<TId>;

    bool TryDecode<TId>(string? base64String, out Cursor<TId>? cursor)
        where TId : struct, IId<TId>;
}