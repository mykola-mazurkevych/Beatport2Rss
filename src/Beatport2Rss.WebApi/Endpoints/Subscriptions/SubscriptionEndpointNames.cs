namespace Beatport2Rss.WebApi.Endpoints.Subscriptions;

internal static class SubscriptionEndpointNames
{
    private const string Prefix = "Subscription";

    public const string CreateArtist = $"{Prefix}{nameof(CreateArtist)}";
    public const string CreateArtistTag = $"{Prefix}{nameof(CreateArtistTag)}";
    public const string CreateLabel = $"{Prefix}{nameof(CreateLabel)}";
    public const string CreateLabelTag = $"{Prefix}{nameof(CreateLabelTag)}";
    public const string DeleteArtistTag = $"{Prefix}{nameof(DeleteArtistTag)}";
    public const string DeleteArtistTags = $"{Prefix}{nameof(DeleteArtistTags)}";
    public const string DeleteLabelTag = $"{Prefix}{nameof(DeleteLabelTag)}";
    public const string DeleteLabelTags = $"{Prefix}{nameof(DeleteLabelTags)}";
    public const string GetArtist = $"{Prefix}{nameof(GetArtist)}";
    public const string GetLabel = $"{Prefix}{nameof(GetLabel)}";
}