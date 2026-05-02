using Beatport2Rss.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackNameValueConverter() :
    ValueConverter<TrackName, string>(
        trackName => trackName.Value,
        value => TrackName.Create(value));