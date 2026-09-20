using System.Text.Json;

namespace Beatport2Rss.Common.EntityFrameworkCore.Entities;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    public Guid Id { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }

    public string Type { get; private set; } = null!;
    public JsonDocument Payload { get; private set; } = null!;
    
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int ProcessAttempts { get; private set; }
    public string? LastError { get; private set; }

    public static InboxMessage Create(
        Guid id,
        DateTimeOffset occurredAt,
        string type,
        string payload,
        DateTimeOffset receivedAt) =>
        new()
        {
            Id = id,
            OccurredAt = occurredAt,
            Type = type,
            Payload = JsonDocument.Parse(payload),
            ReceivedAt = receivedAt,
        };
    
    public void MarkProcessed(DateTimeOffset processedAt)
    {
        ProcessedAt = processedAt;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        ProcessAttempts++;
        LastError = error;
    }
}