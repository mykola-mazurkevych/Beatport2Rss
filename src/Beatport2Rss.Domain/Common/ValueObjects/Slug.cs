using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct Slug : IValueObject
{
    public const char Delimiter = '-';
    public const int SuffixLength = 4;
    public const int MaxLength = 200;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Slug Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.SlugEmpty));
        Guard.Against.InvalidInput(value,
            nameof(value),
            predicate: SlugIsValid,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.SlugInvalid));
        Guard.Against.StringTooLong(value,
            MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.SlugTooLong));

        return new Slug(value);
    }

    public bool Equals(Slug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(Slug left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(Slug left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, Slug right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, Slug right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public static implicit operator string(Slug value) => value.Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;

    private static bool SlugIsValid(string value) =>
        value.Contains(Delimiter, StringComparison.OrdinalIgnoreCase) &&
        value.Split(Delimiter).Length == 2 &&
        value.Split(Delimiter)[1].Length == SuffixLength;
}