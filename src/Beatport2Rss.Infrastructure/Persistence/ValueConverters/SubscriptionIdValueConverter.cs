using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionIdValueConverter() : ValueConverter<SubscriptionId, int>(subscriptionId => subscriptionId.Value, value => SubscriptionId.Create(value));