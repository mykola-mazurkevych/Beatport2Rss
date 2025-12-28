namespace Beatport2Rss.Domain.Common.Exceptions;

public abstract class ConflictException(string title, string? detail = null) : Exception
{
    public string Title { get; } = title;
    public string? Detail { get; } = detail;
}