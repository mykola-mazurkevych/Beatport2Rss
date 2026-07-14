using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Users;

public readonly record struct Password :
    IValueObject
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Password Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(Password)} cannot be empty"))
            .MustBeLongerThanOrEqualTo(MinLength, (_, _) => new InvalidValueObjectValueException($"{nameof(Password)} too short"))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(Password)} too long")));

    public bool Equals(Password other) => StringComparer.Ordinal.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}