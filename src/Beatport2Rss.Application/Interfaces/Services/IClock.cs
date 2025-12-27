namespace Beatport2Rss.Application.Interfaces.Services;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}