using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Api.Domain.Users;

public readonly record struct EmailVerificationSecretHash :
    IValueObject
{
    public const int MaxLength = 64;

    private EmailVerificationSecretHash(string value) => Value = value;

    public string Value { get; }

    public static EmailVerificationSecretHash Create([NotNull] string? value)
    {
        if (value is null || value.Length != MaxLength || !value.All(char.IsAsciiHexDigit))
        {
            throw new InvalidValueObjectValueException($"{nameof(EmailVerificationSecretHash)} must be a SHA-256 hexadecimal value");
        }

        return new EmailVerificationSecretHash(value.ToUpperInvariant());
    }

    public bool Equals(EmailVerificationSecretHash other) => StringComparer.Ordinal.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}
