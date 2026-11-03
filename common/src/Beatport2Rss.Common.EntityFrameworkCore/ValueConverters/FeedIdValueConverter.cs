using Beatport2Rss.Common.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class FeedIdValueConverter() :
    ValueConverter<FeedId, Guid>(
        feedId => feedId.Value,
        value => FeedId.Create(value));