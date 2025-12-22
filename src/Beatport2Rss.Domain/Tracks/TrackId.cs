using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tracks;

public readonly record struct TrackId : IValueObject
{
    private TrackId(int value) => Value = value;

    public int Value { get; }

    public static TrackId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.TrackIdInvalid));

        return new TrackId(value);
    }

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}