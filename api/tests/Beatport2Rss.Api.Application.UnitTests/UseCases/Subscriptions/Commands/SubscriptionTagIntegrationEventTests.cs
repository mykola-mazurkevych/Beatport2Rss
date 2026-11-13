using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Api.Domain.Subscriptions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.Subscriptions.Commands;

public sealed class SubscriptionTagIntegrationEventTests
{
    [Fact]
    public async Task CreateSubscriptionTagCommandHandler_ShouldEnqueueCreatedEvent()
    {
        var subscription = CreateSubscription();
        var tagId = TagId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        subscriptionRepository
            .Setup(repository => repository.LoadWithTagsAsync(subscription.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var tagRepository = new Mock<ITagQueryRepository>();
        tagRepository
            .Setup(repository => repository.LoadTagIdAsync(userId, It.IsAny<Slug>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagId);

        var outbox = new Mock<IIntegrationEventOutbox>();
        var handler = new CreateSubscriptionTagCommandHandler(
            tagRepository.Object,
            subscriptionRepository.Object,
            new Mock<IClock>().Object,
            outbox.Object,
            new Mock<IUnitOfWork>().Object);

        await handler.Handle(
            new CreateSubscriptionTagCommand(userId, subscription.Slug, Slug.Create("techno")),
            CancellationToken.None);

        outbox.Verify(
            value => value.EnqueueAsync(
                It.Is<SubscriptionTagCreated>(integrationEvent =>
                    integrationEvent.SubscriptionId == subscription.Id.Value &&
                    integrationEvent.TagId == tagId.Value),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteSubscriptionTagCommandHandler_ShouldEnqueueDeletedEvent()
    {
        var subscription = CreateSubscription();
        var tagId = TagId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        subscription.AddTag(tagId);

        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        subscriptionRepository
            .Setup(repository => repository.LoadWithTagsAsync(subscription.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var tagRepository = new Mock<ITagQueryRepository>();
        tagRepository
            .Setup(repository => repository.LoadTagIdAsync(userId, It.IsAny<Slug>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagId);

        var outbox = new Mock<IIntegrationEventOutbox>();
        var handler = new DeleteSubscriptionTagCommandHandler(
            tagRepository.Object,
            subscriptionRepository.Object,
            new Mock<IClock>().Object,
            outbox.Object,
            new Mock<IUnitOfWork>().Object);

        await handler.Handle(
            new DeleteSubscriptionTagCommand(userId, subscription.Slug, Slug.Create("techno")),
            CancellationToken.None);

        outbox.Verify(
            value => value.EnqueueAsync(
                It.Is<SubscriptionTagDeleted>(integrationEvent =>
                    integrationEvent.SubscriptionId == subscription.Id.Value &&
                    integrationEvent.TagId == tagId.Value),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteSubscriptionTagsCommandHandler_ShouldEnqueueDeletedEventForEachTag()
    {
        var subscription = CreateSubscription();
        subscription.AddTag(TagId.Create(Guid.NewGuid()));
        subscription.AddTag(TagId.Create(Guid.NewGuid()));

        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        subscriptionRepository
            .Setup(repository => repository.LoadWithTagsAsync(subscription.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var outbox = new Mock<IIntegrationEventOutbox>();
        var handler = new DeleteSubscriptionTagsCommandHandler(
            subscriptionRepository.Object,
            new Mock<IClock>().Object,
            outbox.Object,
            new Mock<IUnitOfWork>().Object);

        await handler.Handle(
            new DeleteSubscriptionTagsCommand(UserId.Create(Guid.NewGuid()), subscription.Slug),
            CancellationToken.None);

        outbox.Verify(
            value => value.EnqueueAsync(It.IsAny<SubscriptionTagDeleted>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        Assert.Empty(subscription.Tags);
    }

    private static Subscription CreateSubscription() =>
        Subscription.Create(
            SubscriptionId.Create(Guid.NewGuid()),
            DateTimeOffset.UtcNow,
            SubscriptionType.Label,
            SubscriptionName.Create("Label"),
            Slug.Create("label"),
            BeatportId.Create(1),
            BeatportSlug.Create("label"),
            new Uri("https://example.com/image.jpg"),
            null,
            null,
            null);
}
