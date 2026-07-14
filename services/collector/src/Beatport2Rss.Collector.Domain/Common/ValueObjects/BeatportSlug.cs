using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Collector.Domain.Common.ValueObjects;

public readonly record struct BeatportSlug :
    IValueObject
{
    public const int MaxLength = 200;

    private BeatportSlug(string value) => Value = value;

    public string Value { get; }

    public static BeatportSlug Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(BeatportSlug)} cannot be empty"))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(BeatportSlug)} is too long")));

    public bool Equals(BeatportSlug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}