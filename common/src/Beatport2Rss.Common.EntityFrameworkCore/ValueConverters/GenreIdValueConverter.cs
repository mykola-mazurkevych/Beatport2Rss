using Beatport2Rss.Common.Domain.Genres;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class GenreIdValueConverter() :
    ValueConverter<GenreId, Guid>(
        genreId => genreId.Value,
        value => GenreId.Create(value));