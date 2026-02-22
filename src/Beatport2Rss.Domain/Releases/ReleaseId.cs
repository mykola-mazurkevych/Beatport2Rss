using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Releases;

public readonly record struct ReleaseId :
    IId<ReleaseId>
{
    private ReleaseId(int value) => Value = value;

    public int Value { get; }

    public static bool operator <(ReleaseId left, ReleaseId right) => left.Value < right.Value;
    public static bool operator >(ReleaseId left, ReleaseId right) => left.Value > right.Value;
    public static bool operator <=(ReleaseId left, ReleaseId right) => left.Value <= right.Value;
    public static bool operator >=(ReleaseId left, ReleaseId right) => left.Value >= right.Value;

    public static ReleaseId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.ReleaseIdInvalid)));

    public int CompareTo(ReleaseId other) => Value.CompareTo(other.Value);
    public bool Equals(ReleaseId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}