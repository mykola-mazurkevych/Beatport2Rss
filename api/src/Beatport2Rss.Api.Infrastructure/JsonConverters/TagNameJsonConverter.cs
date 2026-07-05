using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Api.Domain.Tags;

namespace Beatport2Rss.Api.Infrastructure.JsonConverters;

internal sealed class TagNameJsonConverter :
    JsonConverter<TagName>
{
    public override TagName Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        TagName.Create(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        TagName value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}