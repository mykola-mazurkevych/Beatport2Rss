// ReSharper disable NotAccessedPositionalProperty.Local

using System.Text.Json;

using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Services;

using MicrosoftOptions = Microsoft.Extensions.Options.Options;

using Moq;

using RabbitMQ.Client;

using Xunit;

namespace Beatport2Rss.Common.Messaging.UnitTests.Services;

public sealed class RabbitMqPublisherTests
{
    private const string QueueName = "test-queue";
    private const string DeadLetterSuffix = "dead-letter";
    private const string DeadLetterQueueName = $"{QueueName}-{DeadLetterSuffix}";

    private readonly Mock<IConnectionFactory> _connectionFactoryMock = new();
    private readonly Mock<IConnection> _connectionMock = new();
    private readonly Mock<IModel> _modelMock = new();

    private readonly QueueOptions _queueOptions = new()
    {
        DeadLetterSuffix = DeadLetterSuffix,
        Queues = new Dictionary<string, string>
        {
            [nameof(TestMessage)] = QueueName,
        },
    };

    public RabbitMqPublisherTests()
    {
        _connectionFactoryMock.Setup(f => f.CreateConnection()).Returns(_connectionMock.Object);
        _connectionMock.Setup(c => c.CreateModel()).Returns(_modelMock.Object);
        _modelMock.Setup(m => m.CreateBasicProperties()).Returns(new Mock<IBasicProperties>().Object);
    }

    private RabbitMqPublisher CreatePublisher() =>
        new(_connectionFactoryMock.Object,
            MicrosoftOptions.Create(_queueOptions),
            MicrosoftOptions.Create(new JsonSerializerOptions()));

    [Fact]
    public async Task PublishAsync_WhenQueueIsConfigured_ShouldPublishToCorrectQueue()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("hello"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.BasicPublish(
                exchange: string.Empty,
                QueueName,
                mandatory: It.IsAny<bool>(),
                basicProperties: It.IsAny<IBasicProperties>(),
                body: It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenQueueIsNotConfigured_ShouldThrowInvalidOperationException()
    {
        await using var publisher = CreatePublisher();

        await Assert.ThrowsAsync<InvalidOperationException>(() => publisher.PublishAsync(new UnknownMessage(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PublishAsync_WhenCalledFirstTime_ShouldDeclareQueueAndDeadLetterQueue()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("hello"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.QueueDeclare(
                DeadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null),
            Times.Once);

        _modelMock.Verify(
            m => m.QueueDeclare(
                QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: It.Is<IDictionary<string, object>>(a =>
                    a.ContainsKey("x-dead-letter-exchange") &&
                    a["x-dead-letter-exchange"] is string &&
                    (string)a["x-dead-letter-exchange"] == string.Empty &&
                    a.ContainsKey("x-dead-letter-routing-key") &&
                    a["x-dead-letter-routing-key"] is string &&
                    (string)a["x-dead-letter-routing-key"] == DeadLetterQueueName)),
            Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WhenCalledMultipleTimes_ShouldDeclareQueuesOnlyOnce()
    {
        await using var publisher = CreatePublisher();

        await publisher.PublishAsync(new TestMessage("first"), TestContext.Current.CancellationToken);
        await publisher.PublishAsync(new TestMessage("second"), TestContext.Current.CancellationToken);
        await publisher.PublishAsync(new TestMessage("third"), TestContext.Current.CancellationToken);

        _modelMock.Verify(
            m => m.QueueDeclare(
                queue: It.IsAny<string>(),
                durable: It.IsAny<bool>(),
                exclusive: It.IsAny<bool>(),
                autoDelete: It.IsAny<bool>(),
                arguments: It.IsAny<IDictionary<string, object>>()),
            Times.Exactly(2));
    }

    private sealed record TestMessage(string Value);

    private sealed record UnknownMessage;
}