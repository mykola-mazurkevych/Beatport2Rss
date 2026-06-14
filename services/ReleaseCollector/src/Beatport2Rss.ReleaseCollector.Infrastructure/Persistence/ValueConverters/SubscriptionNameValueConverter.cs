using Beatport2Rss.ReleaseCollector.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionNameValueConverter() :
    ValueConverter<SubscriptionName, string>(
        subscriptionName => subscriptionName.Value,
        value => SubscriptionName.Create(value));