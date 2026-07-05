namespace Beatport2Rss.Api.Application.Interfaces.Services.Misc;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}