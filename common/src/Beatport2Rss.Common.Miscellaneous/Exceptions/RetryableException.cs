namespace Beatport2Rss.Common.Miscellaneous.Exceptions;

public sealed class RetryableException : Exception
{
    public RetryableException()
    {
    }

    public RetryableException(string? message) : base(message)
    {
    }

    public RetryableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}