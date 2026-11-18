using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;

public interface IGenreCommandRepository
{
    Task<IEnumerable<Genre>> FindAllAsync(IEnumerable<BeatportId> beatportIds, CancellationToken cancellationToken = default);

    Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default);
}
