// ReSharper disable NotAccessedPositionalProperty.Local

using System.Text.Json;

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Services;

using MicrosoftOptions = Microsoft.Extensions.Options.Options;

using Moq;

using RabbitMQ.Client;

using Xunit;

namespace Beatport2Rss.Common.Messaging.UnitTests.Services;

public sealed class RabbitMqPublisherTests
{
    private const string ExchangeName = "beatport2rss.events";
    private const string RoutingKey = "test.created";
    private const string DeadLetterSuffix = "dead-letter";

    private readonly Mock<IConnection> _connectionMock = new();
    private readonly Mock<IModel> _modelMock = new();

    private readonly QueueOptions _queueOptions = new()
    {
        DeadLetterSuffix = DeadLetterSuffix,
        ExchangeName = ExchangeName,
        Queues = new Dictionary<string, string>(),
        RoutingKeys = new Dictionary<string, string>
        {
            [nameof(TestMessage)] = RoutingKey,
        },
    };

    public RabbitMqPublisherTests()
    {
        _connectionMock.Setup(c => c.CreateModel()).Returns(_modelMock.Object);
        _modelMock.Setup(m => m.CreateBasicProperties()).Returns(new Mock<IBasicProperties>().Object);
    }

    private RabbitMqPublisher CreatePublisher() =>
        new(new TestRabbitMqConnectionFactory(_connectionMock.Object),
            MicrosoftOptions.Create(_queueOptions),
            MicrosoftOptions.Create(new JsonSerializerOptions()));

    [Fact]
    public async Task PublishAsync_WhenRoutingKeyIsConfigured_ShouldPublishToTopicExchange()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("hello"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                mandatory: It.IsAny<bool>(),
                basicProperties: It.IsAny<IBasicProperties>(),
                body: It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenRoutingKeyIsNotConfigured_ShouldThrowInvalidOperationException()
    {
        await using var publisher = CreatePublisher();

        await Assert.ThrowsAsync<InvalidOperationException>(() => publisher.PublishAsync(new UnknownMessage(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PublishAsync_WhenCalledFirstTime_ShouldDeclareTopicExchange()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("hello"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.ExchangeDeclare(
                ExchangeName,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenCalledMultipleTimes_ShouldDeclareExchangeOnlyOnce()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("first"), TestContext.Current.CancellationToken);
        await publisher.PublishAsync(new TestMessage("second"), TestContext.Current.CancellationToken);
        await publisher.PublishAsync(new TestMessage("third"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.ExchangeDeclare(
                exchange: It.IsAny<string>(),
                type: It.IsAny<string>(),
                durable: It.IsAny<bool>(),
                autoDelete: It.IsAny<bool>(),
                arguments: It.IsAny<IDictionary<string, object>>()),
            Times.Once);
    }

    private sealed record TestMessage(string Value);

    private sealed record UnknownMessage;

    private sealed class TestRabbitMqConnectionFactory(IConnection connection) :
        IRabbitMqConnectionFactory
    {
        public IConnection CreateConnection() =>
            connection;
    }
}