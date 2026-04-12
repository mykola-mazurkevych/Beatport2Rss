using System.Text.Json;

using Beatport2Rss.Infrastructure.QueryModels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence;

internal sealed class SubscriptionTagQueryModelsValueConverter() :
    ValueConverter<IReadOnlyCollection<SubscriptionTagQueryModel>, string>(
        tags => JsonSerializer.Serialize(tags),
        json => JsonSerializer.Deserialize<IReadOnlyCollection<SubscriptionTagQueryModel>>(json) ?? Array.Empty<SubscriptionTagQueryModel>());