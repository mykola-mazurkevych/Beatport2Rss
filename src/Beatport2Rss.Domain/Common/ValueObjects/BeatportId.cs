using System.Globalization;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportId
{
    private BeatportId(int value) => Value = value;

    public int Value { get; }

    public static BeatportId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.BeatportIdInvalid));

        return new BeatportId(value);
    }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public override int GetHashCode() => Value.GetHashCode();
}