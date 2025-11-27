namespace Beatport2Rss.SharedKernel;

public sealed class InvalidValueObjectValueException : Exception
{
    public InvalidValueObjectValueException()
    {
    }

    public InvalidValueObjectValueException(string message)
        : base(message)
    {
    }

    public InvalidValueObjectValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}