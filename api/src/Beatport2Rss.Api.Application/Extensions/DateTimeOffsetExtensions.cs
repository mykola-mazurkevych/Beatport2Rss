namespace Beatport2Rss.Api.Application.Extensions;

internal static class DateTimeOffsetExtensions
{
    extension(DateTimeOffset dateTimeOffset)
    {
        public DateTimeOffset ToHttpDate() =>
            DateTimeOffset.FromUnixTimeSeconds(dateTimeOffset.ToUnixTimeSeconds());
    }
}