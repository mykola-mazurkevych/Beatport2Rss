using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportSlug
{
    public const int MaxLength = 200;

    private BeatportSlug(string value) => Value = value;

    public string Value { get; }

    public static BeatportSlug Create(string value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.BeatportSlugEmpty));
        Guard.Against.StringTooLong(value,
            MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.BeatportSlugTooLong));

        return new BeatportSlug(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public bool Equals(Slug other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
}