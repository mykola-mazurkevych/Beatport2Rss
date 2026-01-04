#pragma warning disable CA1822 // Mark members as static

namespace Beatport2Rss.Application.Results;

public readonly record struct InvalidCredentials
{
    public string Detail => "Invalid email address or password.";
}