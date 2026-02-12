using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Infrastructure.JsonConverters;

internal sealed class SlugJsonConverter :
    JsonConverter<Slug>
{
    public override Slug Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Slug.Create(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        Slug value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}