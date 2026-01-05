#pragma warning disable CA1822 // Mark members as static

namespace Beatport2Rss.Application.Results;

public readonly record struct InactiveUser
{
    public string Detail => "User is not active.";
}