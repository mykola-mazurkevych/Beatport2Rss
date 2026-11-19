using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;

internal sealed class GenreCommandRepository(DbSet<Genre> genres) :
        CommandRepository<Genre, GenreId>(genres),
    IGenreCommandRepository
{
    private readonly DbSet<Genre> _genres = genres;

    public Task<bool> ExistsAsync(BeatportId beatportId, CancellationToken cancellationToken = default) =>
        _genres.Local.Any(genre => genre.BeatportId == beatportId)
            ? Task.FromResult(true)
            : ExistsAsync(
                genre => genre.BeatportId == beatportId,
                cancellationToken);
}