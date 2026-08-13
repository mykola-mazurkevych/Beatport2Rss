using Beatport2Rss.Builder.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionNameValueConverter() :
    ValueConverter<SubscriptionName, string>(
        subscriptionName => subscriptionName.Value,
        value => SubscriptionName.Create(value));