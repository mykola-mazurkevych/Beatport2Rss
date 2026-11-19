using Beatport2Rss.Collector.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;

public interface IGenreCommandRepository
{
    Task<bool> ExistsAsync(BeatportId beatportId, CancellationToken cancellationToken = default);

    Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default);
}