namespace Beatport2Rss.WebApi.Endpoints.Sessions;

internal static class SessionEndpointNames
{
    private const string Prefix = "Session";

    public const string Create = $"{Prefix}{nameof(Create)}";
    public const string DeleteAll = $"{Prefix}{nameof(DeleteAll)}";
    public const string DeleteById = $"{Prefix}{nameof(DeleteById)}";
    public const string DeleteCurrent = $"{Prefix}{nameof(DeleteCurrent)}";
    public const string GetCurrent = $"{Prefix}{nameof(GetCurrent)}";
    public const string UpdateCurrent = $"{Prefix}{nameof(UpdateCurrent)}";
}