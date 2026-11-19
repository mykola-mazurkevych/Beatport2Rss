using Beatport2Rss.Builder.Application.UseCases.Genres.Commands;
using Beatport2Rss.Common.IntegrationEvents.Genres;

namespace Beatport2Rss.Builder.Application.UseCases.Genres.IntegrationEvents;

public static class GenreCreatedHandler
{
    public static CreateGenreCommand Handle(GenreCreated genreCreated) =>
        new(genreCreated.GenreId,
            genreCreated.OccurredAt,
            genreCreated.BeatportId,
            genreCreated.Name);
}