using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Entities;

[Index(nameof(OccurredAt))]
public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    [Key]
    public Guid Id { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }

    [Required]
    [MaxLength(200)]
    public string Type { get; private set; } = null!;
    [Required]
    [Column(TypeName = "jsonb")]
    public JsonDocument Payload { get; private set; } = null!;
    
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int ProcessAttempts { get; private set; }
    [MaxLength(4_000)]
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