using Beatport2Rss.Domain.Countries;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class CountryCodeValueConverter() :
    ValueConverter<CountryCode, string>(
        countryCode => countryCode.Value,
        value => CountryCode.Create(value));