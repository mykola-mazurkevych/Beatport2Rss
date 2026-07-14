using Beatport2Rss.Collector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackNameValueConverter() :
    ValueConverter<TrackName, string>(
        trackName => trackName.Value,
        value => TrackName.Create(value));