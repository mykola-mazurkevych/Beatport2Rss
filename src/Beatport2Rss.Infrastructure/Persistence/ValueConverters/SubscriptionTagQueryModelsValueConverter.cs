using System.Text.Json;

using Beatport2Rss.Infrastructure.JsonConverters;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionTagQueryModelsValueConverter() :
    ValueConverter<IReadOnlyList<SubscriptionTagQueryModel>, string>(
        subscriptionTagQueryModels => JsonSerializer.Serialize(subscriptionTagQueryModels, JsonSerializerOptions),
        json => JsonSerializer.Deserialize<IReadOnlyList<SubscriptionTagQueryModel>>(json, JsonSerializerOptions) ??
                new List<SubscriptionTagQueryModel>())
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new SlugJsonConverter(),
            new TagNameJsonConverter(),
            new UserIdJsonConverter(),
        }
    };
}