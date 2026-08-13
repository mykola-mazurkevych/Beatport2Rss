using Beatport2Rss.Builder.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionIdValueConverter() :
    ValueConverter<SubscriptionId, Guid>(
        subscriptionId => subscriptionId.Value,
        value => SubscriptionId.Create(value));