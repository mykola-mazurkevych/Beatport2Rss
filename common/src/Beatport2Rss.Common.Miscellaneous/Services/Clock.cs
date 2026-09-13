using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Common.Miscellaneous.Services;

internal sealed class Clock :
    IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}