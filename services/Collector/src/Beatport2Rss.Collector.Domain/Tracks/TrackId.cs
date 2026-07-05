using Beatport2Rss.Collector.Domain.Common.Constants;
using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Collector.Domain.Tracks;

public readonly record struct TrackId :
    IId<TrackId>
{
    private TrackId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(TrackId left, TrackId right) => left.Value < right.Value;
    public static bool operator >(TrackId left, TrackId right) => left.Value > right.Value;
    public static bool operator <=(TrackId left, TrackId right) => left.Value <= right.Value;
    public static bool operator >=(TrackId left, TrackId right) => left.Value >= right.Value;

    public static TrackId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.TrackIdEmpty)));

    public int CompareTo(TrackId other) => Value.CompareTo(other.Value);
    public bool Equals(TrackId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}