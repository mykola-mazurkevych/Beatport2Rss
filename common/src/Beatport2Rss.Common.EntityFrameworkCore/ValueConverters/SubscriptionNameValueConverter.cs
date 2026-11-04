using Beatport2Rss.Common.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class SubscriptionNameValueConverter() :
    ValueConverter<SubscriptionName, string>(
        subscriptionName => subscriptionName.Value,
        value => SubscriptionName.Create(value));