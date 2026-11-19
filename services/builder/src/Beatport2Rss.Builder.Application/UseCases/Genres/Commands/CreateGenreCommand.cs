using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Genres.Commands;

public sealed record CreateGenreCommand(
    Guid GenreId,
    DateTimeOffset CreatedAt,
    int BeatportId,
    string Name);

public sealed class CreateGenreCommandHandler(
    IGenreCommandRepository genreCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateGenreCommand command, CancellationToken cancellationToken)
    {
        var genreId = GenreId.Create(command.GenreId);
        if (await genreCommandRepository.ExistsAsync(genreId, cancellationToken))
        {
            return;
        }

        var genre = Genre.Create(
            genreId,
            command.CreatedAt,
            BeatportId.Create(command.BeatportId),
            GenreName.Create(command.Name));
        await genreCommandRepository.AddAsync(genre, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}