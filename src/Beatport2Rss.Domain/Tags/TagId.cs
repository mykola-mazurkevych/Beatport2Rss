using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tags;

public readonly record struct TagId : IValueObject
{
    private TagId(int value) => Value = value;

    public int Value { get; }

    public static TagId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.TagIdInvalid));

        return new TagId(value);
    }

    public bool Equals(TagId other) => Value == other.Value;

    public static bool operator ==(TagId left, int right) => left.Value == right;
    public static bool operator !=(TagId left, int right) => left.Value != right;
    public static bool operator ==(int left, TagId right) => left == right.Value;
    public static bool operator !=(int left, TagId right) => left != right.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}