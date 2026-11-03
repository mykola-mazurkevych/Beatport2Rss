using System.Text.Json.Serialization;

namespace Beatport2Rss.Common.Domain.Feeds;

[JsonConverter(typeof(JsonStringEnumConverter<FeedStatus>))]
public enum FeedStatus
{
    Active,
    Inactive,
}