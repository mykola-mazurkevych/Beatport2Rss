using Beatport2Rss.Builder.Domain.Artists;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class ArtistNameValueConverter() :
    ValueConverter<ArtistName, string>(
        aartistName => aartistName.Value,
        value => ArtistName.Create(value));