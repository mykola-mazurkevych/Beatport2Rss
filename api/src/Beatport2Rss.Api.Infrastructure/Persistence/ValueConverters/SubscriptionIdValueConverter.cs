using Beatport2Rss.Api.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionIdValueConverter() :
    ValueConverter<SubscriptionId, int>(
        subscriptionId => subscriptionId.Value,
        value => SubscriptionId.Create(value));