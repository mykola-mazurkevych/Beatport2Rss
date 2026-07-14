using Beatport2Rss.Collector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackIdValueConverter() :
    ValueConverter<TrackId, Guid>(
        trackId => trackId.Value,
        value => TrackId.Create(value));