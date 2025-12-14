using System.Globalization;

using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Common;

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