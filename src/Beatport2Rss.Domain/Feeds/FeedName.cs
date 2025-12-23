using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Feeds;

public readonly record struct FeedName : IValueObject
{
    public const int MaxLength = 200;

    private FeedName(string value) => Value = value;

    public string Value { get; }

    public static FeedName Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.FeedNameEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.FeedNameTooLong));

        return new FeedName(value);
    }

    public bool Equals(FeedName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(FeedName left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(FeedName left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, FeedName right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, FeedName right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public static implicit operator string(FeedName value) => value.Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}