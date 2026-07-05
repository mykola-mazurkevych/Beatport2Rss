using Beatport2Rss.ReleaseCollector.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistNameValueConverter() :
    ValueConverter<ArtistName, string>(
        aartistName => aartistName.Value,
        value => ArtistName.Create(value));