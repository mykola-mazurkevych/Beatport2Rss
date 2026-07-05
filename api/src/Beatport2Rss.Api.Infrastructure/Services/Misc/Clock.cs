using Beatport2Rss.Api.Application.Interfaces.Services.Misc;

namespace Beatport2Rss.Api.Infrastructure.Services.Misc;

internal sealed class Clock :
    IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}