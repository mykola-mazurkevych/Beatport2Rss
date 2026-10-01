namespace Beatport2Rss.Builder.Jobs.Options;

public sealed record BuilderJobOptions
{
    public required int ReleasesRetentionInDays { get; init; }

    public required string DeleteStaleReleasesCronSchedule { get; init; }
    public required string BuildRSSFeedJobCronSchedule { get; init; }
}