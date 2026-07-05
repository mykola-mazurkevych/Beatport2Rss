using Beatport2Rss.Api.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class SubscriptionNameValueConverter() :
    ValueConverter<SubscriptionName, string>(
        subscriptionName => subscriptionName.Value,
        value => SubscriptionName.Create(value));