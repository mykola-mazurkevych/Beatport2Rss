namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public sealed class EmailConfirmationOptions
{
    public const string SectionName = "EmailConfirmation";

    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string PublicWebBaseUrl { get; set; } = string.Empty;
    public TimeSpan Lifetime { get; set; } = TimeSpan.FromHours(24);
    public Guid? AdministratorUserId { get; set; }
}
