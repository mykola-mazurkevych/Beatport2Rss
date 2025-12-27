using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct BeatportAccessToken : IValueObject
{
    public const int MaxLength = 100;

    private BeatportAccessToken(string value) => Value = value;

    public string Value { get; }

    public static BeatportAccessToken Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.BeatportAccessTokenEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.BeatportAccessTokenTooLong)));

    public bool Equals(BeatportAccessToken other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public static bool operator ==(BeatportAccessToken left, string? right) => StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator !=(BeatportAccessToken left, string? right) => !StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator ==(string? left, BeatportAccessToken right) => StringComparer.Ordinal.Equals(left, right.Value);
    public static bool operator !=(string? left, BeatportAccessToken right) => !StringComparer.Ordinal.Equals(left, right.Value);

    public static implicit operator string(BeatportAccessToken value) => value.Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}