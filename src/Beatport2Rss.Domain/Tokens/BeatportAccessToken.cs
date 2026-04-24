using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct BeatportAccessToken :
    IValueObject
{
    public const int MaxLength = 2000;

    private BeatportAccessToken(string value) => Value = value;

    public string Value { get; }

    public static BeatportAccessToken Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.BeatportAccessTokenEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.BeatportAccessTokenTooLong)));

    public bool Equals(BeatportAccessToken other) => StringComparer.Ordinal.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}