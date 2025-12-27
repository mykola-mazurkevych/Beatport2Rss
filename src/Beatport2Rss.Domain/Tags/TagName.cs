using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tags;

public readonly record struct TagName : IValueObject
{
    public const int MaxLength = 200;

    private TagName(string value) => Value = value;

    public string Value { get; }

    public static TagName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.TagNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.TagNameTooLong)));

    public bool Equals(TagName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(TagName left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(TagName left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, TagName right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, TagName right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public static implicit operator string(TagName tagName) => tagName.Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}