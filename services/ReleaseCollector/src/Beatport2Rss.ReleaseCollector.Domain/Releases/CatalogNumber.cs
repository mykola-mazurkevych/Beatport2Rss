using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.ReleaseCollector.Domain.Common.Constants;
using Beatport2Rss.SharedKernel.Exceptions;
using Beatport2Rss.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.ReleaseCollector.Domain.Releases;

public readonly record struct CatalogNumber :
    IValueObject
{
    public const int MaxLength = 100;

    private CatalogNumber(string value) => Value = value;

    public string Value { get; }

    public static CatalogNumber Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.CatalogNumberEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.CatalogNumberTooLong)));

    public bool Equals(CatalogNumber other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}