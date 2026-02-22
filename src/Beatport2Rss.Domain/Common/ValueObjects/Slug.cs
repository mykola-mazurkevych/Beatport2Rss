using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct Slug :
    IValueObject, IParsable<Slug>
{
    public const char Delimiter = '-';
    public const int SuffixLength = 4;
    public const int MaxLength = 200;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Slug Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.SlugEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.SlugTooLong)));

    public static Slug Parse(string s, IFormatProvider? provider) =>
        Create(s);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Slug result)
    {
        result = string.IsNullOrWhiteSpace(s) || s.Length > MaxLength
            ? default
            : Create(s);

        return result != default;
    }

    public bool Equals(Slug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}