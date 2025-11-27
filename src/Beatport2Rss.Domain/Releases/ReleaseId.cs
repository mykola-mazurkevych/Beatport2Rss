using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Releases;

public readonly record struct ReleaseId : IValueObject
{
    private ReleaseId(int value) => Value = value;

    public int Value { get; }

    public static ReleaseId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.ReleaseIdInvalid));

        return new ReleaseId(value);
    }

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}