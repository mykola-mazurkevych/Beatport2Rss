using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.ReleaseCollector.Domain.Common.Constants;
using Beatport2Rss.SharedKernel.Exceptions;
using Beatport2Rss.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.ReleaseCollector.Domain.Labels;

public readonly record struct LabelName :
    IValueObject
{
    public const int MaxLength = 200;

    private LabelName(string value) => Value = value;

    public string Value { get; }

    public static LabelName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.LabelNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.LabelNameTooLong)));

    public bool Equals(LabelName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}