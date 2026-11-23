namespace Beatport2Rss.Common.Email.Options;

internal sealed record ResendOptions
{
    public required string ApiToken { get; init; }
}