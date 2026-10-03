namespace Beatport2Rss.Common.Messaging.Jobs.Options;

internal sealed record OutboxJobOptions
{
    public required int OutboxMessagesRetentionInDays { get; init; }

    public required string DeleteStaleOutboxMessagesCronSchedule { get; init; }
}