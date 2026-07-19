using Beatport2Rss.Collector.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionIdValueConverter() :
    ValueConverter<SubscriptionId, Guid>(
        subscriptionId => subscriptionId.Value,
        value => SubscriptionId.Create(value));