using Beatport2Rss.Builder.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistIdValueConverter() :
    ValueConverter<ArtistId, Guid>(
        artistId => artistId.Value,
        value => ArtistId.Create(value));