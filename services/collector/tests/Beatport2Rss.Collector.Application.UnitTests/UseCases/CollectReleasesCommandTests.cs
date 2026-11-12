using System.Globalization;

using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases;

public sealed class CollectReleasesCommandTests
{
    [Theory]
    [InlineData("en-US", false, "2026-11-02:2026-11-12")]
    [InlineData("uk-UA", true, "2026-10-30:2026-11-12")]
    public async Task HandleAsync_FormatsPublishDatesIndependentlyOfCulture(
        string cultureName,
        bool previouslyRefreshed,
        string expectedPublishDate)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var cancellationToken = TestContext.Current.CancellationToken;
            var today = new DateOnly(2026, 11, 12);
            var subscription = Subscription.Create(
                SubscriptionId.Create(Guid.NewGuid()),
                DateTimeOffset.UtcNow,
                SubscriptionType.Artist,
                BeatportId.Create(42));
            if (previouslyRefreshed)
            {
                subscription.RecordRefreshAttempt(new DateOnly(2026, 10, 30));
                subscription.MarkRefreshed();
            }

            var clock = new Mock<IClock>();
            clock.SetupGet(value => value.UtcToday).Returns(today);
            var subscriptions = new Mock<ISubscriptionCommandRepository>();
            subscriptions.Setup(value => value.FindNextToRefreshAsync(cancellationToken)).ReturnsAsync(subscription);
            IReadOnlyDictionary<string, string?>? queryParameters = null;
            var client = new Mock<IBeatportClient>();
            client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                    It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
                .Callback<IReadOnlyDictionary<string, string?>, CancellationToken>((parameters, _) => queryParameters = parameters)
                .ReturnsAsync(Result.Ok<IReadOnlyCollection<int>>([]));
            var releases = new Mock<IReleaseCommandRepository>();
            releases.Setup(value => value.FindAllAsync(It.IsAny<IEnumerable<BeatportId>>(), cancellationToken))
                .ReturnsAsync(Array.Empty<Release>());
            var handler = new CollectReleasesCommandHandler(
                releases.Object,
                subscriptions.Object,
                new UnusedSubscriptionService(),
                client.Object,
                Mock.Of<IIntegrationEventOutbox>(),
                Mock.Of<IUnitOfWork>(),
                clock.Object,
                NullLogger<CollectReleasesCommand>.Instance);

            await handler.HandleAsync(new CollectReleasesCommand(), cancellationToken);

            Assert.NotNull(queryParameters);
            Assert.Equal(expectedPublishDate, queryParameters["publish_date"]);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    private sealed class UnusedSubscriptionService : ISubscriptionService
    {
        public Task<Result> EnsureSubscriptionExistsAsync(
            SubscriptionType subscriptionType,
            BeatportId beatportId,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("No subscriptions should be created when there are no releases.");
    }
}