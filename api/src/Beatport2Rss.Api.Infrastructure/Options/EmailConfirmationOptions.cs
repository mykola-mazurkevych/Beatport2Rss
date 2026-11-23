namespace Beatport2Rss.Api.Infrastructure.Options;

public sealed record EmailConfirmationOptions
{
    public bool Enabled { get; init; }
    public string ApiKey { get; init; } = string.Empty;
    public string SenderAddress { get; init; } = string.Empty;
    public string SenderName { get; init; } = string.Empty;
    public Uri? PublicWebBaseUrl { get; init; }
}