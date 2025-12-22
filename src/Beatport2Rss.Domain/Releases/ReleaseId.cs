using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Releases;

public readonly record struct ReleaseId : IValueObject
{
    private ReleaseId(int value) => Value = value;

    public int Value { get; }

    public static ReleaseId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.ReleaseIdInvalid));

        return new ReleaseId(value);
    }

    public bool Equals(ReleaseId other) => Value == other.Value;

    public static bool operator ==(ReleaseId left, int right) => left.Value == right;
    public static bool operator !=(ReleaseId left, int right) => left.Value != right;
    public static bool operator ==(int left, ReleaseId right) => left == right.Value;
    public static bool operator !=(int left, ReleaseId right) => left != right.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}