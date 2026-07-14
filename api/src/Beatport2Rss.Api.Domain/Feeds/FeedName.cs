using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Feeds;

public readonly record struct FeedName :
    IValueObject
{
    public const int MaxLength = 200;

    private FeedName(string value) => Value = value;

    public string Value { get; }

    public static FeedName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(FeedName)} cannot be empty"))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(FeedName)} is too long")));

    public bool Equals(FeedName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}