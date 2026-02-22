#pragma warning disable CA2225 // Operator overloads have named alternates

namespace Beatport2Rss.SharedKernel.Common;

public interface IId<in TSelf> :
    IValueObject, IComparable<TSelf>
    where TSelf : IId<TSelf>
{
    static abstract bool operator <(TSelf left, TSelf right);
    static abstract bool operator >(TSelf left, TSelf right);
    static abstract bool operator <=(TSelf left, TSelf right);
    static abstract bool operator >=(TSelf left, TSelf right);
}