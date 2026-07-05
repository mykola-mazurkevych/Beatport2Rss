using Beatport2Rss.ReleaseCollector.Domain.Common.Constants;
using Beatport2Rss.SharedKernel.Exceptions;
using Beatport2Rss.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.ReleaseCollector.Domain.Artists;

public readonly record struct ArtistId :
    IId<ArtistId>
{
    private ArtistId(Guid value) => Value = value;

    public Guid Value { get; }

    public static ArtistId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.ArtistIdEmpty)));

    public static bool operator <(ArtistId left, ArtistId right) => left.Value < right.Value;
    public static bool operator >(ArtistId left, ArtistId right) => left.Value > right.Value;
    public static bool operator <=(ArtistId left, ArtistId right) => left.Value >= right.Value;
    public static bool operator >=(ArtistId left, ArtistId right) => left.Value >= right.Value;

    public int CompareTo(ArtistId other) => Value.CompareTo(other.Value);
    public bool Equals(ArtistId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}