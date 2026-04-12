using System.Text.Json;

using Beatport2Rss.Infrastructure.Persistence.QueryModels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionTagQueryModelsValueConverter() :
    ValueConverter<IReadOnlyCollection<SubscriptionTagQueryModel>, string>(
        subscriptionTagQueryModels => JsonSerializer.Serialize(subscriptionTagQueryModels),
        json => JsonSerializer.Deserialize<IReadOnlyCollection<SubscriptionTagQueryModel>>(json) ?? Array.Empty<SubscriptionTagQueryModel>());