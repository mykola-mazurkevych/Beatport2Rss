using Beatport2Rss.ReleaseCollector.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionIdValueConverter() :
    ValueConverter<SubscriptionId, Guid>(
        subscriptionId => subscriptionId.Value,
        value => SubscriptionId.Create(value));