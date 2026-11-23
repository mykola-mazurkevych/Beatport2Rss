using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public static class EmailConfirmationIdentifier
{
    public static EmailConfirmationId Generate() => EmailConfirmationId.Create(Guid.NewGuid());

    public static EmailConfirmationId? Parse(string? identifier) =>
        Guid.TryParseExact(identifier, "D", out var value) && value != Guid.Empty
            ? EmailConfirmationId.Create(value)
            : null;
}
