using System.Text.Json.Serialization;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed class SendConfirmationEmail
{
    [JsonConstructor]
    public SendConfirmationEmail(string identifier) => Identifier = identifier;

    [JsonInclude]
    internal string? Identifier { get; }

    public override string ToString() => nameof(SendConfirmationEmail);
}
