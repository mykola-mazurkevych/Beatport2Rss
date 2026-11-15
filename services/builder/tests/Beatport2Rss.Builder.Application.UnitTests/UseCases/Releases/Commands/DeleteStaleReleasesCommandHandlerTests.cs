using System.Linq.Expressions;

using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Releases.Commands;

public sealed class DeleteStaleReleasesCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesReleasesOlderThanRetentionBoundary()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var now = new DateTimeOffset(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);
        var clock = new Mock<IClock>();
        clock.SetupGet(value => value.UtcNow).Returns(now);
        var repository = new Mock<IReleaseCommandRepository>(MockBehavior.Strict);
        Expression<Func<Release, bool>>? deletionPredicate = null;
        repository.Setup(value => value.DeleteAsync(
                It.IsAny<Expression<Func<Release, bool>>>(),
                cancellationToken))
            .Callback<Expression<Func<Release, bool>>, CancellationToken>((predicate, _) => deletionPredicate = predicate)
            .Returns(Task.CompletedTask);
        var handler = new DeleteStaleReleasesCommandHandler(clock.Object, repository.Object);

        await handler.Handle(new DeleteStaleReleasesCommand(10), cancellationToken);

        repository.Verify(value => value.DeleteAsync(
            It.IsAny<Expression<Func<Release, bool>>>(),
            cancellationToken), Times.Once);
        Assert.NotNull(deletionPredicate);
        var isStale = deletionPredicate.Compile();
        Assert.True(isStale(CreateRelease(now.AddDays(-11))));
        Assert.False(isStale(CreateRelease(now.AddDays(-10))));
        Assert.False(isStale(CreateRelease(now.AddDays(-9))));
    }

    private static Release CreateRelease(DateTimeOffset createdAt) =>
        Release.Create(
            ReleaseId.Create(Guid.NewGuid()),
            createdAt,
            BeatportId.Create(100),
            BeatportSlug.Create("release"),
            new Uri("https://example.com/release"),
            ReleaseName.Create("Release"),
            new DateOnly(2026, 11, 15),
            SubscriptionId.Create(Guid.NewGuid()),
            CatalogNumber.Create("CAT-1"),
            new Uri("https://example.com/image"),
            0,
            null,
            [],
            []);
}