using Beatport2Rss.ReleaseCollector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackIdValueConverter() :
    ValueConverter<TrackId, Guid>(
        trackId => trackId.Value,
        value => TrackId.Create(value));