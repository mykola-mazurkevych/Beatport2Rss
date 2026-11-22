using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.Domain.Constants;
using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.EmailConfirmations;

public readonly record struct SecretHash :
    IValueObject
{
    public const int MaxLength = MaxLengthConstants.SecretHash;

    private SecretHash(string value) => Value = value;

    public string Value { get; }

    public static SecretHash Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(SecretHash)} must be a SHA-256 hexadecimal value"))
            .MustHaveLength(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(SecretHash)} must be a SHA-256 hexadecimal value"))
            .MustBeHexadecimal(_ => new InvalidValueObjectValueException($"{nameof(SecretHash)} must be a SHA-256 hexadecimal value")));

    public bool Equals(SecretHash other) => StringComparer.Ordinal.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}