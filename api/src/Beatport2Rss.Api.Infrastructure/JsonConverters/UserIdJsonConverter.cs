using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Infrastructure.JsonConverters;

internal sealed class UserIdJsonConverter :
    JsonConverter<UserId>
{
    public override UserId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        UserId.Create(reader.GetGuid());

    public override void Write(
        Utf8JsonWriter writer,
        UserId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}