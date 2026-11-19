using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Genres;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IGenreCommandRepository
{
    Task<Genre?> FindAsync(GenreId genreId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Genre>> FindAllAsync(IEnumerable<GenreId> genreIds, CancellationToken cancellationToken = default);

    Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default);
}
