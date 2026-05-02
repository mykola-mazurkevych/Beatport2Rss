using Beatport2Rss.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class CatalogNumberValueConverter() :
    ValueConverter<CatelogNumber, string>(
        catalogNumber => catalogNumber.Value,
        value => CatelogNumber.Create(value));