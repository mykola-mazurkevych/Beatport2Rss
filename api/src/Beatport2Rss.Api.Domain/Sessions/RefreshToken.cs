using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Sessions;

public readonly record struct RefreshToken :
    IValueObject
{
    public const int Length = 43;

    private RefreshToken(string value) => Value = value;

    public string Value { get; }

    public static RefreshToken Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(RefreshToken)} cannot be empty"))
            .MustHaveLength(Length, (_, _) => new InvalidValueObjectValueException($"{nameof(RefreshToken)} must be {Length} characters long")));

    public bool Equals(RefreshToken other) => StringComparer.Ordinal.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}