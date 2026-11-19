using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IGenreCommandRepository
{
    Task<bool> ExistsAsync(GenreId genreId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Genre>> FindAllAsync(IEnumerable<BeatportId> beatportIds, CancellationToken cancellationToken = default);

    Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default);
}