using Beatport2Rss.Builder.Application.UseCases.Genres.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Genres;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Genres.IntegrationEvents;

public sealed class GenreCreatedHandlerTests
{
    [Fact]
    public void Handle_MapsGenreCreatedEventToCommand()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var genreId = Guid.NewGuid();
        var command = GenreCreatedHandler.Handle(
            new GenreCreated(Guid.NewGuid(), occurredAt, genreId, 5, "House"));

        Assert.Equal(genreId, command.GenreId);
        Assert.Equal(occurredAt, command.CreatedAt);
        Assert.Equal(5, command.BeatportId);
        Assert.Equal("House", command.Name);
    }
}
