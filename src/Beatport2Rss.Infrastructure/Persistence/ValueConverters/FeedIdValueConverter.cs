using Beatport2Rss.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class FeedIdValueConverter() :
    ValueConverter<FeedId, Guid>(
        feedId => feedId.Value,
        value => FeedId.Create(value));