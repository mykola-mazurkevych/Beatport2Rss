using Beatport2Rss.Builder.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class FeedIdValueConverter() :
    ValueConverter<FeedId, Guid>(
        feedId => feedId.Value,
        value => FeedId.Create(value));