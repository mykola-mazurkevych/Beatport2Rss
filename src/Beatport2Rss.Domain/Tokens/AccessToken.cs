using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct AccessToken : IValueObject
{
    private const int MaxLength = 100;

    private AccessToken(string value) => Value = value;

    public string Value { get; }

    public static AccessToken Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.AccessTokenEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.AccessTokenTooLong));

        return new AccessToken(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    public bool Equals(AccessToken other) => StringComparer.Ordinal.Equals(Value, other.Value);
}