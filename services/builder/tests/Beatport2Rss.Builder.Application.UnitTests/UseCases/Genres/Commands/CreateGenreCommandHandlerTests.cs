using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Genres.Commands;
using Beatport2Rss.Builder.Domain.Genres;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Genres.Commands;

public sealed class CreateGenreCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesGenreFromCommand()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var genreId = Guid.NewGuid();
        Genre? addedGenre = null;
        var repository = new Mock<IGenreCommandRepository>();
        repository.Setup(value => value.ExistsAsync(GenreId.Create(genreId), cancellationToken))
            .ReturnsAsync(false);
        repository.Setup(value => value.AddAsync(It.IsAny<Genre>(), cancellationToken))
            .Callback<Genre, CancellationToken>((genre, _) => addedGenre = genre)
            .ReturnsAsync((Genre genre, CancellationToken _) => genre);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new CreateGenreCommandHandler(repository.Object, unitOfWork.Object);

        await handler.HandleAsync(
            new CreateGenreCommand(genreId, DateTimeOffset.UtcNow, 5, "House"),
            cancellationToken);

        Assert.NotNull(addedGenre);
        Assert.Equal(GenreId.Create(genreId), addedGenre.Id);
        Assert.Equal(5, addedGenre.BeatportId.Value);
        Assert.Equal("House", addedGenre.Name.Value);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenGenreAlreadyExists_DoesNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var genreId = GenreId.Create(Guid.NewGuid());
        var repository = new Mock<IGenreCommandRepository>();
        repository.Setup(value => value.ExistsAsync(genreId, cancellationToken)).ReturnsAsync(true);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var handler = new CreateGenreCommandHandler(repository.Object, unitOfWork.Object);

        await handler.HandleAsync(
            new CreateGenreCommand(genreId.Value, DateTimeOffset.UtcNow, 5, "House"),
            cancellationToken);

        repository.Verify(value => value.AddAsync(It.IsAny<Genre>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
