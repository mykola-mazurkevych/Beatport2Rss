using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class GenreCommandRepository(DbSet<Genre> genres) :
    CommandRepository<Genre, GenreId>(genres),
    IGenreCommandRepository
{
    Task<Genre?> IGenreCommandRepository.FindAsync(GenreId genreId, CancellationToken cancellationToken) =>
        base.FindAsync(genreId, cancellationToken);

    public Task<IEnumerable<Genre>> FindAllAsync(
        IEnumerable<GenreId> genreIds,
        CancellationToken cancellationToken = default) =>
        FindAllAsync(genre => genreIds.Contains(genre.Id), cancellationToken);
}
