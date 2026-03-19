namespace Beatport2Rss.WebApi.Endpoints.Users;

internal static class UserEndpointNames
{
    private const string Prefix = "User";

    public const string Create = $"{Prefix}{nameof(Create)}";
    public const string Delete = $"{Prefix}{nameof(Delete)}";
    public const string DeleteCurrent = $"{Prefix}{nameof(DeleteCurrent)}";
    public const string GetCurrent = $"{Prefix}{nameof(GetCurrent)}";
    public const string UpdateCurrent = $"{Prefix}{nameof(UpdateCurrent)}";
    public const string UpdateCurrentEmailAddress = $"{Prefix}{nameof(UpdateCurrentEmailAddress)}";
    public const string UpdateCurrentPassword = $"{Prefix}{nameof(UpdateCurrentPassword)}";
    public const string UpdateCurrentStatus = $"{Prefix}{nameof(UpdateCurrentStatus)}";
}