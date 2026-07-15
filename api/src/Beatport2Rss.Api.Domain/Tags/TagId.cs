using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Tags;

public readonly record struct TagId :
    IId<TagId>
{
    private TagId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(TagId left, TagId right) => left.Value < right.Value;
    public static bool operator >(TagId left, TagId right) => left.Value > right.Value;
    public static bool operator <=(TagId left, TagId right) => left.Value <= right.Value;
    public static bool operator >=(TagId left, TagId right) => left.Value >= right.Value;

    public static TagId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(TagId)} cannot be empty")));

    public int CompareTo(TagId other) => Value.CompareTo(other.Value);
    public bool Equals(TagId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}