using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class GenreCommandRepository(DbSet<Genre> genres) :
    IGenreCommandRepository
{
    public async Task<IEnumerable<Genre>> FindAllAsync(
        IEnumerable<BeatportId> beatportIds,
        CancellationToken cancellationToken = default)
    {
        var ids = beatportIds.ToHashSet();
        var persisted = await genres
            .Where(genre => ids.Contains(genre.BeatportId))
            .ToListAsync(cancellationToken);
        return persisted
            .Concat(genres.Local.Where(genre => ids.Contains(genre.BeatportId)))
            .DistinctBy(genre => genre.BeatportId)
            .ToArray();
    }

    public async Task<Genre> AddAsync(Genre genre, CancellationToken cancellationToken = default) =>
        (await genres.AddAsync(genre, cancellationToken)).Entity;
}
