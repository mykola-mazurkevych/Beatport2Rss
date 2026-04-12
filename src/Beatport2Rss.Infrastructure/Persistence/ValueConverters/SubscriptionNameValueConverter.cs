using Beatport2Rss.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionNameValueConverter() :
    ValueConverter<SubscriptionName, string>(
        subscriptionName => subscriptionName.Value,
        value => SubscriptionName.Create(value));