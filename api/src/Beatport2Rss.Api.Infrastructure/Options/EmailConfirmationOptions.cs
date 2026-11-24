namespace Beatport2Rss.Api.Infrastructure.Options;

public sealed record EmailConfirmationOptions
{
    public bool Enabled { get; init; }
    public string TemplateId { get; init; } = string.Empty;
    public Uri? PublicWebBaseUrl { get; init; }
}