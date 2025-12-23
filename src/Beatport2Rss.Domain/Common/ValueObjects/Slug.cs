using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly partial record struct Slug : IValueObject
{
    public const char Delimiter = '-';
    public const int SuffixLength = 4;
    public const int MaxLength = 200;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Slug Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.SlugEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.SlugTooLong))
            .MustMatch(SlugRegex(), (_, _) => new InvalidValueObjectValueException(ExceptionMessages.SlugInvalid)));

    public bool Equals(Slug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(Slug left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(Slug left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, Slug right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, Slug right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public static implicit operator string(Slug value) => value.Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;

    [GeneratedRegex("^[^-]+-[^-]{4}$", RegexOptions.IgnoreCase)]
    private static partial Regex SlugRegex();
}