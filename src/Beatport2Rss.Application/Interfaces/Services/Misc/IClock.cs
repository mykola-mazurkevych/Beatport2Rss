namespace Beatport2Rss.Application.Interfaces.Services.Misc;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}