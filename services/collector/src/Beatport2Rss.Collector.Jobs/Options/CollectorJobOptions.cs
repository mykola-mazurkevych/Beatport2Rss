namespace Beatport2Rss.Collector.Jobs.Options;

internal sealed record CollectorJobOptions
{
    public required string CollectReleasesCronSchedule { get; init; }
}