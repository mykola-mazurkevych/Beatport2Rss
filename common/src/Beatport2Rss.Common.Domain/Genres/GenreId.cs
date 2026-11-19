using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Common.Domain.Genres;

public readonly record struct GenreId :
    IId<GenreId>
{
    private GenreId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(GenreId left, GenreId right) => left.Value < right.Value;
    public static bool operator >(GenreId left, GenreId right) => left.Value > right.Value;
    public static bool operator <=(GenreId left, GenreId right) => left.Value <= right.Value;
    public static bool operator >=(GenreId left, GenreId right) => left.Value >= right.Value;

    public static GenreId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(GenreId)} cannot be empty")));

    public int CompareTo(GenreId other) => Value.CompareTo(other.Value);
    public bool Equals(GenreId other) => Value.Equals(other.Value);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}