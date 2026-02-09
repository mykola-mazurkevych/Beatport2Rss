using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.WebApi.Converters;

internal sealed class RefreshTokenJsonConverter : JsonConverter<RefreshToken>
{
    public override RefreshToken Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        RefreshToken.Create(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        RefreshToken value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}