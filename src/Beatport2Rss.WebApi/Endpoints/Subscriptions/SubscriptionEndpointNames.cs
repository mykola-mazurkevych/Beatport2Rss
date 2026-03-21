namespace Beatport2Rss.WebApi.Endpoints.Subscriptions;

internal static class SubscriptionEndpointNames
{
    private const string Prefix = "Subscription";

    public const string Create = $"{Prefix}{nameof(Create)}";
    public const string CreateTag = $"{Prefix}{nameof(CreateTag)}";
    public const string DeleteTag = $"{Prefix}{nameof(DeleteTag)}";
    public const string DeleteTags = $"{Prefix}{nameof(DeleteTags)}";
    public const string Get = $"{Prefix}{nameof(Get)}";
}