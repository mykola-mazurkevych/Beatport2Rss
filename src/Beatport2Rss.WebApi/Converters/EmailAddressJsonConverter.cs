using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.WebApi.Converters;

internal sealed class EmailAddressJsonConverter : JsonConverter<EmailAddress>
{
    public override EmailAddress Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        EmailAddress.Create(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        EmailAddress value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}