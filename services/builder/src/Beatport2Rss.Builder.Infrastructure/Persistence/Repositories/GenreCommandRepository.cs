using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class GenreCommandRepository(DbSet<Genre> genres) :
    CommandRepository<Genre, GenreId>(genres),
    IGenreCommandRepository
{
    public Task<IEnumerable<Genre>> FindAllAsync(
        IEnumerable<BeatportId> beatportIds,
        CancellationToken cancellationToken = default) =>
        FindAllAsync(genre => beatportIds.Contains(genre.BeatportId), cancellationToken);
}