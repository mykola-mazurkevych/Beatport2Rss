using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.Domain.Constants;
using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Builder.Domain.Genres;

public readonly record struct GenreName :
    IValueObject
{
    public const int MaxLength = MaxLengthConstants.GenreName;

    private GenreName(string value) => Value = value;

    public string Value { get; }

    public static GenreName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(GenreName)} cannot be empty"))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(GenreName)} is too long")));

    public bool Equals(GenreName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}