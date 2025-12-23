using System.Globalization;

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

    public bool Equals(TrackId other) => Value == other.Value;

    public static bool operator ==(TrackId left, int right) => left.Value == right;
    public static bool operator !=(TrackId left, int right) => left.Value != right;
    public static bool operator ==(int left, TrackId right) => left == right.Value;
    public static bool operator !=(int left, TrackId right) => left != right.Value;

    public static implicit operator int(TrackId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}