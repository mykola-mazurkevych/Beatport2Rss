using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.EmailConfirmations.Queries;

public sealed class GetEmailConfirmationQueryHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 24, 12, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(-1, false, true)]
    [InlineData(0, false, true)]
    [InlineData(1, false, false)]
    [InlineData(-1, true, true)]
    [InlineData(1, true, false)]
    public async Task Handle_WhenConfirmationExists_ShouldReturnItsState(
        int expiryOffsetSeconds,
        bool confirmed,
        bool expired)
    {
        var id = EmailConfirmationId.Create(Guid.NewGuid());
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IEmailConfirmationQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(id, cancellationToken))
            .ReturnsAsync(new EmailConfirmationReadModel(
                id,
                Now.AddSeconds(expiryOffsetSeconds),
                confirmed ? Now : null));
        var clock = new Mock<IClock>(MockBehavior.Strict);
        clock.SetupGet(value => value.UtcNow).Returns(Now);

        var result = await new GetEmailConfirmationQueryHandler(repository.Object, clock.Object)
            .Handle(new GetEmailConfirmationQuery(id), cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(expired, result.Value.IsExpired);
        Assert.Equal(confirmed, result.Value.IsConfirmed);
    }

    [Fact]
    public async Task Handle_WhenConfirmationDoesNotExist_ShouldReturnNotFound()
    {
        var id = EmailConfirmationId.Create(Guid.NewGuid());
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IEmailConfirmationQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(id, cancellationToken))
            .ReturnsAsync((EmailConfirmationReadModel?)null);
        var clock = new Mock<IClock>(MockBehavior.Strict);

        var result = await new GetEmailConfirmationQueryHandler(repository.Object, clock.Object)
            .Handle(new GetEmailConfirmationQuery(id), cancellationToken);

        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
    }
}
