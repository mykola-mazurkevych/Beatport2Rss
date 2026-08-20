using System.Text.Json;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Outbox;

internal sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public Guid Id { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Type { get; private set; } = null!;
    public JsonDocument Payload { get; private set; } = null!;
    public DateTimeOffset? PublishedAt { get; private set; }
    public int PublishAttempts { get; private set; }
    public string? LastError { get; private set; }

    public static OutboxMessage Create(
        Guid id,
        DateTimeOffset occurredAt,
        string type,
        string payload) =>
        new()
        {
            Id = id,
            OccurredAt = occurredAt,
            Type = type,
            Payload = JsonDocument.Parse(payload),
        };

    public void MarkPublished(DateTimeOffset publishedAt)
    {
        PublishedAt = publishedAt;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        PublishAttempts++;
        LastError = error;
    }
}