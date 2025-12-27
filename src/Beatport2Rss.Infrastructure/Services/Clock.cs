using Beatport2Rss.Application.Interfaces.Services;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class Clock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}