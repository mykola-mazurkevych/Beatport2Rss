using Beatport2Rss.ReleaseCollector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackNameValueConverter() :
    ValueConverter<TrackName, string>(
        trackName => trackName.Value,
        value => TrackName.Create(value));