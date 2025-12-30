#pragma warning disable CA1822 // Mark members as static

namespace Beatport2Rss.Application.Types;

public readonly record struct Success;

public readonly record struct Success<TValue>(in TValue Value);

public readonly record struct EmailAddressAlreadyTaken
{
    public EmailAddressAlreadyTaken(string emailAddress)
    {
        Detail = $"Email address {emailAddress} already taken.";
    }

    public string Detail { get; }
}

public readonly record struct InactiveUser
{
    public string Detail => "User is not active.";
}

public readonly record struct InvalidCredentials
{
    public string Detail => "Invalid email address or password.";
}

public readonly record struct None;

public readonly record struct NotFound(string Detail);

public readonly record struct Unprocessable(string Detail);

public readonly record struct ValidationFailed(IDictionary<string, string[]> Errors);