using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

public sealed class UriValueConverter() :
    ValueConverter<Uri, string>(
        uri => uri.ToString(),
        value => new Uri(value));