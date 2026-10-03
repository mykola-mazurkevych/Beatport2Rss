namespace Beatport2Rss.Common.Messaging.Jobs.Options;

internal sealed record InboxJobOptions
{
    public required int InboxMessagesRetentionInDays { get; init; }

    public required string DeleteStaleInboxMessagesCronSchedule { get; init; }
}