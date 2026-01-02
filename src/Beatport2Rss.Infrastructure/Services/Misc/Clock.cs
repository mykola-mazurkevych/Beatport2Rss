using Beatport2Rss.Application.Interfaces.Services.Misc;

namespace Beatport2Rss.Infrastructure.Services.Misc;

internal sealed class Clock :
    IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}