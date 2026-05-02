using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Countries;

public readonly record struct CountryCode :
    IId<CountryCode>
{
    public const int Length = 2;

    private CountryCode(string value) => Value = value;

    public string Value { get; }

    public static bool operator <(CountryCode left, CountryCode right) => left.CompareTo(right) < 0;
    public static bool operator >(CountryCode left, CountryCode right) => left.CompareTo(right) > 0;
    public static bool operator <=(CountryCode left, CountryCode right) => left.CompareTo(right) <= 0;
    public static bool operator >=(CountryCode left, CountryCode right) => left.CompareTo(right) >= 0;

    public static CountryCode Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.CountryCodeEmpty))
            .MustHaveLength(Length, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.CountryCodeIncorrectLength)));

    public int CompareTo(CountryCode other) => Value.CompareTo(other.Value, StringComparison.OrdinalIgnoreCase);
    public bool Equals(CountryCode other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}