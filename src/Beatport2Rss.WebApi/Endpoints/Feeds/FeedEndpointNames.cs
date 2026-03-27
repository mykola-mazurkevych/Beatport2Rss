namespace Beatport2Rss.WebApi.Endpoints.Feeds;

internal static class FeedEndpointNames
{
    private const string Prefix = "Feed";

    public const string Create = $"{Prefix}{nameof(Create)}";
    public const string CreateSubscription = $"{Prefix}{nameof(CreateSubscription)}";
    public const string Delete = $"{Prefix}{nameof(Delete)}";
    public const string DeleteSubscription = $"{Prefix}{nameof(DeleteSubscription)}";
    public const string Get = $"{Prefix}{nameof(Get)}";
    public const string List = $"{Prefix}{nameof(List)}";
    public const string Update = $"{Prefix}{nameof(Update)}";
    public const string UpdateStatus = $"{Prefix}{nameof(UpdateStatus)}";
}