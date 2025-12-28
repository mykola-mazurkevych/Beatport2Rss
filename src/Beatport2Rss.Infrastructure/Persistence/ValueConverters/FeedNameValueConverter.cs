using Beatport2Rss.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class FeedNameValueConverter() :
    ValueConverter<FeedName, string>(
        feedName => feedName.Value,
        value => FeedName.Create(value));