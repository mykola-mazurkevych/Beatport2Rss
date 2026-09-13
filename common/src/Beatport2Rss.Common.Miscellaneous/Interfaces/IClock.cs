namespace Beatport2Rss.Common.Miscellaneous.Interfaces;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}