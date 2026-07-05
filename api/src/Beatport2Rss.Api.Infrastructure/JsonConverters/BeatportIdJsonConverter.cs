using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Infrastructure.JsonConverters;

internal sealed class BeatportIdJsonConverter :
    JsonConverter<BeatportId>
{
    public override BeatportId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        BeatportId.Create(reader.GetInt32());

    public override void Write(
        Utf8JsonWriter writer,
        BeatportId value,
        JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}