using Beatport2Rss.ReleaseCollector.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistIdValueConverter() :
    ValueConverter<ArtistId, Guid>(
        artistId => artistId.Value,
        value => ArtistId.Create(value));