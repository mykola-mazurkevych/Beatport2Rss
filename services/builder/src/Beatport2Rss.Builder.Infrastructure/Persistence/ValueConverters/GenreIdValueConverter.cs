using Beatport2Rss.Builder.Domain.Genres;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class GenreIdValueConverter() :
    ValueConverter<GenreId, Guid>(
        genreId => genreId.Value,
        value => GenreId.Create(value));
