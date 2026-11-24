namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public sealed class EmailConfirmationOptions
{
    public const string SectionName = "EmailConfirmation";

    public bool Enabled { get; set; }
    public TimeSpan Lifetime { get; set; } = TimeSpan.FromHours(24);
}
