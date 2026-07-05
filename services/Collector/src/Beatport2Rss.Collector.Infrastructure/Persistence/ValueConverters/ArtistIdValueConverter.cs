using Beatport2Rss.Collector.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistIdValueConverter() :
    ValueConverter<ArtistId, Guid>(
        artistId => artistId.Value,
        value => ArtistId.Create(value));