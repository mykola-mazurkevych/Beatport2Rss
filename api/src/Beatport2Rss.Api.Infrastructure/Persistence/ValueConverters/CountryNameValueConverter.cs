using Beatport2Rss.Api.Domain.Countries;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class CountryNameValueConverter() :
    ValueConverter<CountryName, string>(
        countryName => countryName.Value,
        value => CountryName.Create(value));