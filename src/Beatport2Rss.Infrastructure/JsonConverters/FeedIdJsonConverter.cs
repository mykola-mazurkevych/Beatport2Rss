using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Infrastructure.JsonConverters;

internal sealed class FeedIdJsonConverter :
    JsonConverter<FeedId>
{
    public override FeedId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        FeedId.Create(reader.GetGuid());

    public override void Write(
        Utf8JsonWriter writer,
        FeedId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}