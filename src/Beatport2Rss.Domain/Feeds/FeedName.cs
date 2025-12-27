using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Feeds;

public readonly record struct FeedName : IValueObject
{
    public const int MaxLength = 200;

    private FeedName(string value) => Value = value;

    public string Value { get; }

    public static FeedName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.FeedNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.FeedNameTooLong)));

    public bool Equals(FeedName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(FeedName left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(FeedName left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, FeedName right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, FeedName right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public static implicit operator string(FeedName feedName) => feedName.Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}