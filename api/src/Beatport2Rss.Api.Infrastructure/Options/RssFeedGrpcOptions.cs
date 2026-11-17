namespace Beatport2Rss.Api.Infrastructure.Options;

public sealed record RssFeedGrpcOptions
{
    public required Uri BuilderGrpcUri { get; init; }
}