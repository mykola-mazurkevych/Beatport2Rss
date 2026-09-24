namespace Beatport2Rss.Common.Messaging.Options;

public sealed record OutboxDispatcherOptions
{
    public required int BatchSize { get; init; }
    public required int DispatchIntervalInSeconds { get; init; }
}