using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Builder.Domain.Labels;

public readonly record struct LabelId :
    IId<LabelId>
{
    private LabelId(Guid value) => Value = value;

    public Guid Value { get; }

    public static LabelId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(LabelId)} cannot be empty")));

    public static bool operator <(LabelId left, LabelId right) => left.Value < right.Value;
    public static bool operator >(LabelId left, LabelId right) => left.Value > right.Value;
    public static bool operator <=(LabelId left, LabelId right) => left.Value >= right.Value;
    public static bool operator >=(LabelId left, LabelId right) => left.Value >= right.Value;

    public int CompareTo(LabelId other) => Value.CompareTo(other.Value);
    public bool Equals(LabelId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}