using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.Messaging.Persistence.Entities;

[Index(nameof(OccurredAt))]
public sealed class OutboxMessage
{
    private OutboxMessage()
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

    public DateTimeOffset? PublishedAt { get; private set; }
    public int PublishAttempts { get; private set; }
    [MaxLength(4_000)]
    public string? LastError { get; private set; }

    [NotMapped]
    public bool IsPublished => PublishedAt.HasValue;

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