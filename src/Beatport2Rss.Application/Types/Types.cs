namespace Beatport2Rss.Application.Types;

public readonly record struct Created;

public readonly record struct EmailAddressAlreadyTaken
{
    public EmailAddressAlreadyTaken(string emailAddress)
    {
        Detail = $"Email address {emailAddress} already taken.";
    }

    public string Detail { get; }
}

public readonly record struct None;

public readonly record struct ValidationFailed(IDictionary<string, string[]> Errors);