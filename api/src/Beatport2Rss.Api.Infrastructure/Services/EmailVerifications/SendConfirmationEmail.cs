using System.Text.Json.Serialization;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailVerifications;

internal sealed class SendConfirmationEmail
{
    [JsonConstructor]
    public SendConfirmationEmail(string identifier) => Identifier = identifier;

    [JsonInclude]
    internal string? Identifier { get; }

    public override string ToString() => nameof(SendConfirmationEmail);
}
