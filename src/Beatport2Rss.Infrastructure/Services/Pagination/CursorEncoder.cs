using System.Text.Json;

using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Pagination;
using Beatport2Rss.Domain.Common.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Infrastructure.Services.Pagination;

internal sealed class CursorEncoder(
    IOptions<JsonSerializerOptions> options) :
    ICursorEndcoder
{
    private readonly JsonSerializerOptions _options = options.Value;

    public string Encode<TId>(Cursor<TId> cursor)
        where TId : struct, IValueObject
    {
        var json = JsonSerializer.Serialize(cursor, _options);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return Base64UrlTextEncoder.Encode(bytes);
    }

    public Cursor<TId>? Decode<TId>(string? cursor)
        where TId : struct, IValueObject
    {
        if (string.IsNullOrEmpty(cursor))
        {
            return null;
        }

        var bytes = Base64UrlTextEncoder.Decode(cursor);
        var json = System.Text.Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<Cursor<TId>>(json, _options);
    }
}