using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.ReleaseCollector.Domain.Common.Constants;
using Beatport2Rss.SharedKernel.Exceptions;
using Beatport2Rss.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.ReleaseCollector.Domain.Artists;

public readonly record struct ArtistName :
    IValueObject
{
    public const int MaxLength = 200;

    private ArtistName(string value) => Value = value;

    public string Value { get; }

    public static ArtistName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.ArtistNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.ArtistNameTooLong)));

    public bool Equals(ArtistName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}