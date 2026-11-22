using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Beatport.JsonConverters;

public sealed class EmptyStringToNullUriJsonConverter :
    JsonConverter<Uri>
{
    public override Uri? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            ? uri
            : new Uri($"https://{value}", UriKind.Absolute);
    }

    public override void Write(
        Utf8JsonWriter writer,
        Uri value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.OriginalString);
}