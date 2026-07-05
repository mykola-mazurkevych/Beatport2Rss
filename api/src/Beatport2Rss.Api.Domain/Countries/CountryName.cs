using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Api.Domain.Common.Constants;
using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Countries;

public readonly record struct CountryName :
    IValueObject
{
    public const int MaxLength = 200;

    private CountryName(string value) => Value = value;

    public string Value { get; }

    public static CountryName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.CountryNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.CountryNameTooLong)));

    public bool Equals(CountryName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}