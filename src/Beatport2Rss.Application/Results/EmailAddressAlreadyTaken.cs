namespace Beatport2Rss.Application.Results;

public readonly record struct EmailAddressAlreadyTaken
{
    public EmailAddressAlreadyTaken(string emailAddress)
    {
        Detail = $"Email address {emailAddress} already taken.";
    }

    public string Detail { get; }
}