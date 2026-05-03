using Beatport2Rss.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class CatalogNumberValueConverter() :
    ValueConverter<CatalogNumber, string>(
        catalogNumber => catalogNumber.Value,
        value => CatalogNumber.Create(value));