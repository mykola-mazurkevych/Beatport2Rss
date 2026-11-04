using Beatport2Rss.Common.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class SubscriptionIdValueConverter() :
    ValueConverter<SubscriptionId, Guid>(
        subscriptionId => subscriptionId.Value,
        value => SubscriptionId.Create(value));