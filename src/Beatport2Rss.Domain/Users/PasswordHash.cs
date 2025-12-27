using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly record struct PasswordHash : IValueObject
{
    public const int MaxLength = 500;

    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static PasswordHash Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.PasswordHashEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.PasswordHashTooLong)));

    public bool Equals(PasswordHash other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public static bool operator ==(PasswordHash left, string? right) => StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator !=(PasswordHash left, string? right) => !StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator ==(string? left, PasswordHash right) => StringComparer.Ordinal.Equals(left, right.Value);
    public static bool operator !=(string? left, PasswordHash right) => !StringComparer.Ordinal.Equals(left, right.Value);

    public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}