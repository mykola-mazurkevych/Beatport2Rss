using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Infrastructure.JsonConverters;

internal sealed class SessionIdJsonConverter :
    JsonConverter<SessionId>
{
    public override SessionId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        SessionId.Create(reader.GetGuid());

    public override void Write(
        Utf8JsonWriter writer,
        SessionId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}