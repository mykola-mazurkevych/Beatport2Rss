namespace Beatport2Rss.Common.BeatportTokenProvider.Options;

public sealed record BeatportTokenProviderOptions
{
    public required Uri TokenInterceptorGrpcUri { get; init; }
}