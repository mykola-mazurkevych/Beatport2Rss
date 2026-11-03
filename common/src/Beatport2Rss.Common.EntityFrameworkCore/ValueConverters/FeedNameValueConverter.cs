using Beatport2Rss.Common.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class FeedNameValueConverter() :
    ValueConverter<FeedName, string>(
        feedName => feedName.Value,
        value => FeedName.Create(value));