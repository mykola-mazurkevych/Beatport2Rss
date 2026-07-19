using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Builder.Domain.Releases;

public readonly record struct ReleaseId :
    IId<ReleaseId>
{
    private ReleaseId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(ReleaseId left, ReleaseId right) => left.Value < right.Value;
    public static bool operator >(ReleaseId left, ReleaseId right) => left.Value > right.Value;
    public static bool operator <=(ReleaseId left, ReleaseId right) => left.Value <= right.Value;
    public static bool operator >=(ReleaseId left, ReleaseId right) => left.Value >= right.Value;

    public static ReleaseId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(ReleaseId)} cannot be empty")));

    public int CompareTo(ReleaseId other) => Value.CompareTo(other.Value);
    public bool Equals(ReleaseId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}