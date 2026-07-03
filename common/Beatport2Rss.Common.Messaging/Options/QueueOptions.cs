namespace Beatport2Rss.Common.Messaging.Options;

public sealed record QueueOptions
{
    public required string DeadLetterSuffix { get; init; }
    public required Dictionary<string, string> Queues { get; init; }
}