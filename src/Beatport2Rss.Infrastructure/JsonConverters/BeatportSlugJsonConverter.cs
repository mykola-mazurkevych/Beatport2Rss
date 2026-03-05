using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Infrastructure.JsonConverters;

internal sealed class BeatportSlugJsonConverter :
    JsonConverter<BeatportSlug>
{
    public override BeatportSlug Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        BeatportSlug.Create(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer,
        BeatportSlug value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}