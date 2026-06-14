using Beatport2Rss.ReleaseCollector.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class CatalogNumberValueConverter() :
    ValueConverter<CatalogNumber, string>(
        catalogNumber => catalogNumber.Value,
        value => CatalogNumber.Create(value));