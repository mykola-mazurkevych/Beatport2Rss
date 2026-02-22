using System.Globalization;

using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Tags;

public readonly record struct TagId :
    IId<TagId>
{
    private TagId(int value) => Value = value;

    public int Value { get; }

    public static bool operator <(TagId left, TagId right) => left.Value < right.Value;
    public static bool operator >(TagId left, TagId right) => left.Value > right.Value;
    public static bool operator <=(TagId left, TagId right) => left.Value <= right.Value;
    public static bool operator >=(TagId left, TagId right) => left.Value >= right.Value;

    public static TagId Create(int value) => new(value);

    public int CompareTo(TagId other) => Value.CompareTo(other.Value);
    public bool Equals(TagId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}