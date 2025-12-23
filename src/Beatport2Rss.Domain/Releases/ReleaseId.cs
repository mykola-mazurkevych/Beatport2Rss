using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Releases;

public readonly record struct ReleaseId : IValueObject
{
    private ReleaseId(int value) => Value = value;

    public int Value { get; }

    public static ReleaseId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.ReleaseIdInvalid)));

    public bool Equals(ReleaseId other) => Value == other.Value;

    public static bool operator ==(ReleaseId left, int right) => left.Value == right;
    public static bool operator !=(ReleaseId left, int right) => left.Value != right;
    public static bool operator ==(int left, ReleaseId right) => left == right.Value;
    public static bool operator !=(int left, ReleaseId right) => left != right.Value;

    public static implicit operator int(ReleaseId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}