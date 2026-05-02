using Beatport2Rss.Domain.Countries;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class CountryNameValueConverter() :
    ValueConverter<CountryName, string>(
        countryName => countryName.Value,
        value => CountryName.Create(value));