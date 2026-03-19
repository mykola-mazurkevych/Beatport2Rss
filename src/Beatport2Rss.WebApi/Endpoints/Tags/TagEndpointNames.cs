namespace Beatport2Rss.WebApi.Endpoints.Tags;

internal static class TagEndpointNames
{
    private const string Prefix = "Tag";

    public const string Create = $"{Prefix}{nameof(Create)}";
    public const string Delete = $"{Prefix}{nameof(Delete)}";
    public const string Get = $"{Prefix}{nameof(Get)}";
    public const string List = $"{Prefix}{nameof(List)}";
    public const string UpdateName = $"{Prefix}{nameof(UpdateName)}";
}