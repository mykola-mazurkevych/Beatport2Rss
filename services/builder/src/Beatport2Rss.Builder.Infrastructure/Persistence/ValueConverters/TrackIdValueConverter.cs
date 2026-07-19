using Beatport2Rss.Builder.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackIdValueConverter() :
    ValueConverter<TrackId, Guid>(
        trackId => trackId.Value,
        value => TrackId.Create(value));