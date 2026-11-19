using Beatport2Rss.Builder.Domain.Genres;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class GenreNameValueConverter() :
    ValueConverter<GenreName, string>(
        catalogNumber => catalogNumber.Value,
        value => GenreName.Create(value));