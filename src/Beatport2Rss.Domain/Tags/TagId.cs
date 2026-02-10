using System.Globalization;

using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tags;

public readonly record struct TagId : IValueObject
{
    private TagId(int value) => Value = value;

    public int Value { get; }

    public static TagId Create(int value) => new(value);

    public bool Equals(TagId other) => Value == other.Value;

    public static bool operator ==(TagId left, int right) => left.Value == right;
    public static bool operator !=(TagId left, int right) => left.Value != right;
    public static bool operator ==(int left, TagId right) => left == right.Value;
    public static bool operator !=(int left, TagId right) => left != right.Value;

    public static implicit operator int(TagId tagId) => tagId.Value;
    public static implicit operator string(TagId tagId) => tagId.ToString();

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}