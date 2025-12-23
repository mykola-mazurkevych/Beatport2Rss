using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct AccessToken : IValueObject
{
    public const int MaxLength = 100;

    private AccessToken(string value) => Value = value;

    public string Value { get; }

    public static AccessToken Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.AccessTokenEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.AccessTokenTooLong)));

    public bool Equals(AccessToken other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public static bool operator ==(AccessToken left, string? right) => StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator !=(AccessToken left, string? right) => !StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator ==(string? left, AccessToken right) => StringComparer.Ordinal.Equals(left, right.Value);
    public static bool operator !=(string? left, AccessToken right) => !StringComparer.Ordinal.Equals(left, right.Value);

    public static implicit operator string(AccessToken value) => value.Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}