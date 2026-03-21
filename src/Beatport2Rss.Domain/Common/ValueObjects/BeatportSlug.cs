using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportSlug :
    IValueObject
{
    public const int MaxLength = 200;

    private BeatportSlug(string value) =>
        Value = value;

    public string Value { get; }

    public static BeatportSlug Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.BeatportSlugEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.BeatportSlugTooLong)));

    public bool Equals(BeatportSlug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}