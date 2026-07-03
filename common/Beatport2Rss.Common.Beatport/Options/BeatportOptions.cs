namespace Beatport2Rss.Common.Beatport.Options;

public sealed record BeatportOptions
{
    public required Uri ApiV4BaseUri { get; init; }
    public required Uri WebBaseUri { get; init; }
}