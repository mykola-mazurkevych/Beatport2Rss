using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tracks;

public readonly record struct TrackId :
    IId<TrackId>
{
    private TrackId(int value) => Value = value;

    public int Value { get; }

    public static bool operator <(TrackId left, TrackId right) => left.Value < right.Value;
    public static bool operator >(TrackId left, TrackId right) => left.Value > right.Value;
    public static bool operator <=(TrackId left, TrackId right) => left.Value <= right.Value;
    public static bool operator >=(TrackId left, TrackId right) => left.Value >= right.Value;

    public static TrackId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.TrackIdInvalid)));

    public int CompareTo(TrackId other) => Value.CompareTo(other.Value);
    public bool Equals(TrackId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}