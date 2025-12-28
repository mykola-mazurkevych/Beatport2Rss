using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class UriValueConverter() :
    ValueConverter<Uri, string>(
        uri => uri.ToString(),
        value => new Uri(value));