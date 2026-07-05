using Beatport2Rss.Api.Domain.Feeds;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class FeedIdValueConverter() :
    ValueConverter<FeedId, Guid>(
        feedId => feedId.Value,
        value => FeedId.Create(value));