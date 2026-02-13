using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Infrastructure.JsonConverters;

internal sealed class AccessTokenJsonConverter :
    JsonConverter<AccessToken>
{
    public override AccessToken Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        AccessToken.Create(reader.GetString(), AccessTokenType.Bearer);

    public override void Write(
        Utf8JsonWriter writer,
        AccessToken value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}