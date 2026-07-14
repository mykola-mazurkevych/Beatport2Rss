using Beatport2Rss.Collector.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistNameValueConverter() :
    ValueConverter<ArtistName, string>(
        aartistName => aartistName.Value,
        value => ArtistName.Create(value));