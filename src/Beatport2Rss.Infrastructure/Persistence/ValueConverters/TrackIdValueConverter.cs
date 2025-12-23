using Beatport2Rss.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class TrackIdValueConverter() : ValueConverter<TrackId, int>(trackId => trackId.Value, value => TrackId.Create(value));