using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Tags;

public readonly record struct TagName : IValueObject
{
    private const int MaxLength = 200;

    private TagName(string value) => Value = value;

    public string Value { get; }

    public static TagName Create(string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.TagNameEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.TagNameTooLong));

        return new TagName(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public bool Equals(TagName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
}